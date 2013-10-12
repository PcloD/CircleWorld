using UnityEngine;

public class UniverseCamera : MonoBehaviour
{
    public float cameraDistance = 10;
    public float zoomSpeed = 10;
    public float scale = 1.0f;

    private Camera cam;
    private bool moving;

    private Vector3 movingFromMouse;
    private Vector3 movingFromWorld;

    public void Awake()
    {
        cam = camera;
        Input.simulateMouseWithTouches = true;
    }

    public void Update()
    {
        if (!moving)
        {
            float zoom = Input.GetAxis("Mouse ScrollWheel");
            cameraDistance -= cameraDistance * zoom * zoomSpeed * Time.deltaTime;
        }

        cam.orthographicSize = cameraDistance * scale;

        if (Input.GetMouseButtonDown(0))
        {
            moving = true;
            movingFromMouse = Input.mousePosition;
            movingFromWorld = cam.ScreenToWorldPoint(movingFromMouse);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            moving = false;
        }

        if (moving)
        {
            Vector3 movingToMouse = Input.mousePosition;
            Vector3 movingToWorld = cam.ScreenToWorldPoint(movingToMouse);

            if (movingFromMouse != movingToMouse)
            {
                Vector3 delta = movingToWorld - movingFromWorld;
                delta.z = 0;

                transform.position -= delta;

                movingFromMouse = movingToMouse;
                movingFromWorld = cam.ScreenToWorldPoint(movingFromMouse);
            }
        }

    }
}


