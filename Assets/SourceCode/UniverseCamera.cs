using UnityEngine;

public class UniverseCamera : MonoBehaviour
{
    public float cameraDistance = 10;
    public float zoomSpeed = 10;
    public float scale = 1.0f;

    private Camera cam;

    public void Awake()
    {
        cam = camera;
    }

    public void Update()
    {
        float zoom = Input.GetAxis("Mouse ScrollWheel");
        cameraDistance -= cameraDistance * zoom * zoomSpeed * Time.deltaTime;

        cam.orthographicSize = cameraDistance * scale;
    }
}


