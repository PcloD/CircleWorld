using UnityEngine;

public class UniverseCamera : MonoBehaviour
{
    public float cameraDistance = 10;
    public float zoomSpeed = 10;
    public float scale = 1.0f;

    private Camera cam;
    private bool moving;

    private Vector3 movingFromInputPosition;
    private Vector3 movingFromWorldPosition;

    public void Awake()
    {
        cam = camera;
    }

    public void Update()
    {
        UpdateZoom();

        UpdateMove();
    }

    private void UpdateZoom()
    {
        if (moving)
            return;

        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            //Use touch

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
            //Use touch
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
                delta.z = 0;

                transform.position -= delta;

                movingFromInputPosition = movingToInputPosition;
                movingFromWorldPosition = cam.ScreenToWorldPoint(movingFromInputPosition);
            }
        }

    }
}


