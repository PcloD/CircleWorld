using UnityEngine;

public class UniverseViewCamera : MonoBehaviour
{
    private const float CAMERA_Z = -10;
    
    public float cameraDistance = 10;
    public float zoomSpeed = 10;
    public float scale = 1.0f;

    private Camera cam;
    private Transform trans;
    private bool moving;

    private Vector3 movingFromInputPosition;
    private Vector3 movingFromWorldPosition;
    private int moveTouchFingerId;

    private bool zooming;
    private int zoomingTouchFinger1Id;
    private int zoomingTouchFinger2Id;
    private Vector3 zoomingTouchFinger1FromPosition;
    private Vector3 zoomingTouchFinger2FromPosition;
    
    private TilemapObjectView followingObject;
    
    public TilemapObjectView FollowingObject 
    {
        get { return followingObject; }
        set { followingObject = value; }
    }

    public void Awake()
    {
        trans = transform;
        cam = camera;
        
        trans.position = new Vector3(0, 0, CAMERA_Z);
    }
 
    //Called by AvatarView.OnTilemapObjectUpdated()
    public void UpdatePosition()
    {
        UpdateZoom();
  
        if (followingObject)
            UpdateFollowingObject();
        else
            UpdateMove();
    }
    
    private void UpdateFollowingObject()
    {
        Vector3 newPosition = followingObject.trans.position;
        newPosition.z = CAMERA_Z;
        
        trans.position = newPosition;
        trans.rotation = followingObject.trans.rotation;
        
        scale = followingObject.TilemapObject.Scale;
    }

    private void UpdateZoom()
    {
        if (moving)
            return;

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

        cam.orthographicSize = cameraDistance * scale;
    }

    private void UpdateMove()
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
                    movingFromWorldPosition = cam.ScreenToWorldPoint(movingFromInputPosition);
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
                movingFromWorldPosition = cam.ScreenToWorldPoint(movingFromInputPosition);
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
            Vector3 movingToWorldPosition = cam.ScreenToWorldPoint(movingToInputPosition);

            if (movingFromInputPosition != movingToInputPosition)
            {
                Vector3 delta = movingToWorldPosition - movingFromWorldPosition;
                Vector3 newPosition = trans.position - delta;
                newPosition.z = CAMERA_Z;

                trans.position = newPosition;

                movingFromInputPosition = movingToInputPosition;
                movingFromWorldPosition = cam.ScreenToWorldPoint(movingFromInputPosition);
            }
        }
    }
}
