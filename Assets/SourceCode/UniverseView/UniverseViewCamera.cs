using UnityEngine;
using UniverseEngine;

public class UniverseViewCamera : MonoBehaviour
{
    static public UniverseViewCamera Instance;
    
    private const float CAMERA_Z = -10;
    
    public float cameraDistance = 10;
    public float zoomSpeed = 10;
    public float scale = 1.0f;
    
    public float minCameraDistance = 4;
    public float maxCameraDistance = 4000;
    

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
    
    private Transform followingObject;
    private Vector2 followingObjectPositionDelta = Vector2.zero;
    private float followingObjectScaleDelta = 0.0f;
    private Quaternion followingObjectRotationDelta = Quaternion.identity;
    
    public Transform FollowingObject 
    {
        get 
        { 
            return followingObject; 
        }
        
        set 
        { 
            if (followingObject != value)
            {
                if (followingObject != null && value != null)
                {
                    followingObjectPositionDelta = trans.position - value.position;
                    followingObjectScaleDelta = scale - value.lossyScale.x;
                    followingObjectRotationDelta = trans.rotation * Quaternion.Inverse(value.rotation);
                }
                
                followingObject = value; 
            }
        }
    }

    public void Awake()
    {
        Instance = this;
        
        trans = transform;
        cam = camera;
        
        trans.position = new Vector3(0, 0, CAMERA_Z);
    }
 
    //Called by GameLogic
    public void UpdatePosition()
    {
        //positionVelocity = Vector3.zero;
        //scaleVelocity = 0;
        smoothTime = 0;
        
        if (followingObject)
        {
            if (GameLogic.Instace.State == GameLogicState.PlayingAvatar && AvatarInput.mode == AvatarInputMode.Move ||
                GameLogic.Instace.State == GameLogicState.PlayingShip && ShipInput.mode == ShipInputMode.Move)
            {
                followingObjectPositionDelta = Vector3.SmoothDamp(followingObjectPositionDelta, Vector2.zero, ref followingObjectDeltaPositionVelocity, 0.5f);
                followingObjectScaleDelta = Mathf.SmoothDamp(followingObjectScaleDelta, 0, ref followingObjectDeltaScaleVelocity, 0.5f);
                followingObjectRotationDelta = Quaternion.Slerp(followingObjectRotationDelta, Quaternion.identity, Time.deltaTime * 2.0f);
            }
            
            Vector3 newPosition;
            
            
            if (GameLogic.Instace.State == GameLogicState.PlayingAvatar && AvatarInput.mode == AvatarInputMode.Edit)
            {
                newPosition = followingObject.position + trans.up * followingObjectPositionDelta.y + trans.right * followingObjectPositionDelta.x;
            }
            else
            {
                newPosition = followingObject.position + (Vector3) followingObjectPositionDelta;
            }
            
            newPosition.z = CAMERA_Z;
            
            trans.position = newPosition;
            trans.rotation = followingObject.rotation * followingObjectRotationDelta;
            
            scale = followingObject.lossyScale.x + followingObjectScaleDelta;
        }
    }
    
    //Called by GameLogic
    //private Vector3 positionVelocity;
    //private float scaleVelocity;
    private float smoothTime;
    private Vector3 followingObjectDeltaPositionVelocity;
    private float followingObjectDeltaScaleVelocity;
    
    //Called by GameLogic
    public bool UpdatePositionSmooth()
    {
        smoothTime += Time.deltaTime;
        
        followingObjectPositionDelta = Vector3.SmoothDamp(followingObjectPositionDelta, Vector2.zero, ref followingObjectDeltaPositionVelocity, 0.5f);
        
        if (followingObject)
        {
            Vector3 newPosition = followingObject.position + trans.up * followingObjectPositionDelta.y + trans.right * followingObjectPositionDelta.x;
            newPosition.z = CAMERA_Z;
            
            Quaternion newRotation = followingObject.rotation;
            float newScale = followingObject.lossyScale.x;
            
            //trans.position = Vector3.SmoothDamp(trans.position, newPosition, ref positionVelocity, 0.5f);
            //trans.rotation = Quaternion.RotateTowards(trans.rotation, newRotation, Time.deltaTime * 180.0f);
            //scale = Mathf.SmoothDamp(scale, newScale, ref scaleVelocity, 1.0f);
            
            trans.position = Vector3.Lerp(trans.position, newPosition, smoothTime / 1.0f);
            trans.rotation = Quaternion.Lerp(trans.rotation, newRotation, smoothTime / 1.0f);
            scale = Mathf.Lerp(scale, newScale, smoothTime / 1.0f);
            
            return smoothTime > 1.0f;
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
                cameraDistance -= cameraDistance * zoom * 4;

                zoomingTouchFinger1FromPosition = finger1ToPosition;
                zoomingTouchFinger2FromPosition = finger2ToPosition;
            }

        }
        else
        {
            //Use mouse
            float zoom = Input.GetAxis("Mouse ScrollWheel");
            cameraDistance -= cameraDistance * zoom * zoomSpeed * Time.deltaTime;
        }
        
        cameraDistance = Mathf.Clamp(cameraDistance, minCameraDistance, maxCameraDistance);  

        cam.orthographicSize = cameraDistance * scale;
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
            
            float clickTolerance = (worldPosTolerance - worldPos).magnitude;
            
            ushort closestThingIndex = ushort.MaxValue;
            float closestThingDistance = float.MaxValue;
            
            ThingPosition[] thingsPositions = universeView.Universe.ThingsPositions;
            ushort[] thingsToRender = universeView.Universe.ThingsToRender;
            ushort thingsToRenderAmount = universeView.Universe.ThingsToRenderAmount;
            
            for (ushort i = 0; i < thingsToRenderAmount; i++)
            {
                ThingPosition thingPosition = thingsPositions[thingsToRender[i]];
                
                float distance = (worldPos - new Vector2(thingPosition.x, thingPosition.y)).sqrMagnitude;
                
                if (distance < (thingPosition.radius + clickTolerance) * (thingPosition.radius + clickTolerance) && 
                    distance < closestThingDistance)
                {
                    closestThingIndex = thingsToRender[i];
                    closestThingDistance = distance;
                }
            }
            
            
            
            if (closestThingIndex != ushort.MaxValue)
            {
                PlanetView targetPlanetView = universeView.GetPlanetView(closestThingIndex);
                
                if (universeView.avatarView.UniverseObject.parent != targetPlanetView.TilemapCircle)
                    GameLogic.Instace.TravelToPlanet(targetPlanetView);
            }
        }
    }    
}
