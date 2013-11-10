using UnityEngine;
using UniverseEngine;

[System.Flags]
public enum FollowCameraParameters
{
    None = 0,
    FollowRotation = 1 << 1,
    FollowScale = 1 << 2
}

public class UniverseViewCamera : MonoBehaviour
{
    static public UniverseViewCamera Instance;
    
    private const float CAMERA_Z = -10;
    private const float SMOOTH_TIME = 0.5f;
    private const float ZOOM_SMOOTH_TIME = 0.15f;
    
    public float cameraDistance = 10;
    public float zoomSpeed = 10;
    public float scale = 1.0f;
    
    public float minCameraDistance = 4;
    public float maxCameraDistance = 36000;

    private Camera cam;
    private Transform trans;
    private bool moving;

    private Vector3 movingFromInputPosition;
    private int moveTouchFingerId;

    private bool zooming;
    private int zoomingTouchFinger1Id;
    private int zoomingTouchFinger2Id;
    private Vector3 zoomingTouchFinger1FromPosition;
    private Vector3 zoomingTouchFinger2FromPosition;
    
    private float zoomingCameraDistanceDelta;
    private float zoomingCameraDistanceDeltaVelocity;
    
    #region Follow Object Parameters
    private Transform followingObject;
    private bool followRotation;
    private bool followScale;
    
    private Vector2 followingObjectPositionDelta;
    private Vector3 followingObjectPositionDeltaVelocity;
    
    private float followingObjectScaleDelta;
    private float followingObjectScaleDeltaVelocity;
    
    private float followingObjectCameraDistanceDelta;
    private float followingObjectCameraDistanceDeltaVelocity;
    
    private Quaternion followingObjectRotationDelta = Quaternion.identity;
    
    private float followObjectSmoothTime;
    #endregion
    
    public Transform FollowingObject 
    {
        get 
        { 
            return followingObject; 
        }
    }

    public void Awake()
    {
        Instance = this;
        
        trans = transform;
        cam = camera;
        
        trans.position = new Vector3(0, 0, CAMERA_Z);
    }
    
    public void Update()
    {
        switch(GameLogic.Instace.State)
        {
            case GameLogicState.PlayingAvatar:
            case GameLogicState.PlayingShip:
                UpdatePosition();
                UpdateZoomInput();
                UpdateZoom();
                break;
                
            case GameLogicState.Travelling:
                UpdatePositionSmooth();
                UpdateZoom();
                break;
        }
    }
 
    //Called by GameLogic
    private void UpdatePosition()
    {
        followObjectSmoothTime = 0;
        
        if (followingObject)
        {
            if (GameLogic.Instace.State == GameLogicState.PlayingAvatar && AvatarViewInput.mode == AvatarInputMode.Move ||
                GameLogic.Instace.State == GameLogicState.PlayingShip && ShipViewInput.mode == ShipInputMode.Move)
            {
                followingObjectPositionDelta = Vector3.SmoothDamp(followingObjectPositionDelta, Vector2.zero, ref followingObjectPositionDeltaVelocity, SMOOTH_TIME);
                followingObjectScaleDelta = Mathf.SmoothDamp(followingObjectScaleDelta, 0, ref followingObjectScaleDeltaVelocity, SMOOTH_TIME);
                followingObjectRotationDelta = Quaternion.Slerp(followingObjectRotationDelta, Quaternion.identity, Time.deltaTime * (1.0f / SMOOTH_TIME));
                followingObjectCameraDistanceDelta = Mathf.SmoothDamp(followingObjectCameraDistanceDelta, 0, ref followingObjectCameraDistanceDeltaVelocity, SMOOTH_TIME);
            }
            
            Vector3 newPosition;
            
            if (GameLogic.Instace.State == GameLogicState.PlayingAvatar && AvatarViewInput.mode == AvatarInputMode.Edit)
                newPosition = followingObject.position + trans.up * followingObjectPositionDelta.y + trans.right * followingObjectPositionDelta.x;
            else
                newPosition = followingObject.position + (Vector3) followingObjectPositionDelta;
            
            newPosition.z = CAMERA_Z;
            
            Quaternion newRotation;
            
            if (followRotation)
                newRotation = followingObject.rotation;
            else
                newRotation = Quaternion.identity;
            
            float newScale;
            
            if (followScale)
                newScale = followingObject.lossyScale.x;
            else
                newScale = 1.0f;
            
            trans.position = newPosition;
            trans.rotation = newRotation * followingObjectRotationDelta;
            scale = newScale + followingObjectScaleDelta;
        }
    }
    
    //Called by GameLogic
    private bool UpdatePositionSmooth()
    {
        followObjectSmoothTime += Time.deltaTime;
        
        //followingObjectPositionDelta = Vector3.SmoothDamp(followingObjectPositionDelta, Vector2.zero, ref followingObjectPositionDeltaVelocity, SMOOTH_TIME);
        
        if (followingObject)
        {
            followingObjectPositionDelta = Vector3.Lerp(followingObjectPositionDelta, Vector2.zero, followObjectSmoothTime / 1.0f);
            
            Vector3 newPosition = followingObject.position + trans.up * followingObjectPositionDelta.y + trans.right * followingObjectPositionDelta.x;
            newPosition.z = CAMERA_Z;
            
            Quaternion newRotation = followingObject.rotation;
            float newScale = followingObject.lossyScale.x;
            
            trans.position = Vector3.Lerp(trans.position, newPosition, followObjectSmoothTime / 1.0f);
            trans.rotation = Quaternion.Lerp(trans.rotation, newRotation, followObjectSmoothTime / 1.0f);
            scale = Mathf.Lerp(scale, newScale, followObjectSmoothTime / 1.0f);
            
            return followObjectSmoothTime > 1.0f;
        }
        else
        {
            return true;
        }
    }
    
    public void UpdateZoomInput()
    {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount == 2)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                if (!zooming || touch1.fingerId != zoomingTouchFinger1Id || touch2.fingerId != zoomingTouchFinger2Id)
                {
                    zooming = true;
                    zoomingTouchFinger1Id = touch1.fingerId;
                    zoomingTouchFinger2Id = touch2.fingerId;

                    zoomingTouchFinger1FromPosition = touch1.position;
                    zoomingTouchFinger2FromPosition = touch2.position;
                }
            }
            else
            {
                zooming = false;
            }

            if (zooming)
            {
                Vector3 finger1ToPosition = Input.GetTouch(0).position;
                Vector3 finger2ToPosition = Input.GetTouch(1).position;

                float deltaFrom = (zoomingTouchFinger1FromPosition - zoomingTouchFinger2FromPosition).magnitude;
                float deltaTo = (finger1ToPosition - finger2ToPosition).magnitude;

                float zoom = (deltaTo - deltaFrom) / Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height);
                
                zoomingCameraDistanceDelta -= (cameraDistance + zoomingCameraDistanceDelta) * zoom * 4;

                zoomingTouchFinger1FromPosition = finger1ToPosition;
                zoomingTouchFinger2FromPosition = finger2ToPosition;
            }
        }
        else
        {
            //Use mouse
            float zoom = Input.GetAxis("Mouse ScrollWheel");
            zoomingCameraDistanceDelta -= (cameraDistance + zoomingCameraDistanceDelta) * zoom * zoomSpeed * Time.deltaTime;
        }
    }
    
    private void UpdateZoom()
    {
        float oldDelta = zoomingCameraDistanceDelta;
        zoomingCameraDistanceDelta = Mathf.SmoothDamp(zoomingCameraDistanceDelta, 0.0f, ref zoomingCameraDistanceDeltaVelocity, ZOOM_SMOOTH_TIME);
        cameraDistance += (oldDelta - zoomingCameraDistanceDelta);
        
        cameraDistance = Mathf.Clamp(cameraDistance, minCameraDistance, maxCameraDistance);  

        cam.orthographicSize = (cameraDistance + followingObjectCameraDistanceDelta) * scale;
    }
    
    public void UpdateMove()
    {
        Vector3 movingToInputPosition = movingFromInputPosition;

        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (!moving || moveTouchFingerId != touch.fingerId)
                {
                    moveTouchFingerId = touch.fingerId;
                    moving = true;

                    movingFromInputPosition = touch.position;
                }
            }
            else
            {
                moving = false;
            }

            if (moving)
                movingToInputPosition = Input.GetTouch(0).position;
        }
        else
        {
            //Use mouse
            if (Input.GetMouseButtonDown(0))
            {
                moving = true;
                movingFromInputPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                moving = false;
            }

            if (moving)
                movingToInputPosition = Input.mousePosition;
        }

        if (moving)
        {
            if (movingFromInputPosition != movingToInputPosition)
            {
                Vector3 movingFromWorldPosition = cam.ScreenToWorldPoint(movingFromInputPosition);
                Vector3 movingToWorldPosition = cam.ScreenToWorldPoint(movingToInputPosition);
                
                Vector3 delta = movingToWorldPosition - movingFromWorldPosition;
                
                if (followingObject)
                {
                    float deltaX = Vector3.Dot(delta, trans.right);
                    float deltaY = Vector3.Dot(delta, trans.up);
                    
                    followingObjectPositionDelta -= new Vector2(deltaX, deltaY);
                    
                    Vector3 newPosition = followingObject.position + trans.up * followingObjectPositionDelta.y + trans.right * followingObjectPositionDelta.x;
                    newPosition.z = CAMERA_Z;
                    
                    trans.position = newPosition;
                }

                movingFromInputPosition = movingToInputPosition;
                movingFromWorldPosition = cam.ScreenToWorldPoint(movingFromInputPosition);
            }
        }
    }
    

    private bool travelInput;
    private Vector2 travelInputStartPosition;
    
    public void UpdateClickOnPlanetToTravel(UniverseView universeView)
    {
        bool clickTravel = false;
        Vector2 clickPosition = Vector2.zero;
        
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (!travelInput)
            {
                if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began && !InputAreas.IsInputArea(Input.GetTouch(0).position))
                {
                    travelInput = true;
                    travelInputStartPosition = Input.GetTouch(0).position;
                }
                else
                {
                    travelInput = false;
                }
            }
            else
            {
                if (Input.touchCount == 1)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        if ((travelInputStartPosition - Input.GetTouch(0).position).magnitude < 10)
                        {
                            clickTravel = true;
                            clickPosition = Input.GetTouch(0).position;
                        }
                        
                        travelInput = false;
                    }
                }
                else
                {
                    travelInput = false;
                }
            }
        }
        else
        {
            if (!travelInput)
            {
                if (Input.GetMouseButtonDown(0) && !InputAreas.IsInputArea(Input.mousePosition))
                {
                    travelInput = true;
                    travelInputStartPosition = Input.mousePosition;
                }
                else
                {
                    travelInput = false;
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0))
                {
                    if ((travelInputStartPosition - (Vector2) Input.mousePosition).magnitude < 10)
                    {
                        clickTravel = true;
                        clickPosition = Input.mousePosition;
                    }
                    
                    travelInput = false;
                }
                else if (!Input.GetMouseButton(0))
                {
                    travelInput = false;
                }
            }            
        }
        
        if (clickTravel)
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(clickPosition);
            Vector2 worldPosTolerance = Camera.main.ScreenToWorldPoint(clickPosition + Vector2.right * (Screen.dpi > 0 ? Screen.dpi : 72) / 2.54f); //1 cm tolerance
            
            int clickedThingIndex = universeView.Universe.FindClosestRenderedThing(worldPos, (worldPos - worldPosTolerance).magnitude);
               
            if (clickedThingIndex >= 0)
            {
                PlanetView targetPlanetView = universeView.GetPlanetView((ushort) clickedThingIndex);
                if (universeView.avatarView.UniverseObject.parent != targetPlanetView.TilemapCircle)
                    GameLogic.Instace.TravelToPlanet(targetPlanetView);
            }
        }
    }
    
    public void FollowObject(Transform toFollow, FollowCameraParameters parameters, bool smoothTransition)
    {
        followRotation = (parameters & FollowCameraParameters.FollowRotation) != 0;
        followScale = (parameters & FollowCameraParameters.FollowScale) != 0;
        
        if (followingObject != toFollow)
        {
            if (smoothTransition && followingObject != null && toFollow != null)
            {
                followingObjectPositionDelta = trans.position - toFollow.position;
                
                if (followScale)
                {
                    followingObjectScaleDelta = scale - toFollow.lossyScale.x;
                    
                    followingObjectCameraDistanceDelta = cameraDistance - cameraDistance * (scale / toFollow.lossyScale.x);
                    cameraDistance = cameraDistance * (scale / toFollow.lossyScale.x);
                }
                else
                {
                    followingObjectScaleDelta = scale - 1.0f;
                    
                    followingObjectCameraDistanceDelta = cameraDistance - cameraDistance * (scale / 1.0f);
                    cameraDistance = cameraDistance * (scale / 1.0f);
                }
                
                if (followRotation)
                    followingObjectRotationDelta = trans.rotation * Quaternion.Inverse(toFollow.rotation);
                else
                    followingObjectRotationDelta = trans.rotation * Quaternion.Inverse(Quaternion.identity);
            }
            
            followingObject = toFollow; 
        }
    }
}
