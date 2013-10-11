using UnityEngine;
using System.Collections;

public class Avatar : TilemapObject 
{
    public float jumpSpeed = 7.0f;
    public float walkSpeed = 3.0f;
    public float cameraDistance = 10;
    public float zoomSpeed = 10;

	// Use this for initialization
	
    public override void Start()
    {
        base.Start();

        transform.position = tileMapCircle.GetPositionFromTileCoordinate(tileMapCircle.width / 2, tileMapCircle.height);
        transform.up = tileMapCircle.GetNormalFromPosition(transform.position);
        transform.localScale = Vector3.one * tileMapCircle.GetScaleFromPosition(transform.position);
	}
	
    public override void Update()
    {
        if (Input.GetMouseButton(0))
        {
            int tileX, tileY;
            if (GetTileCoordinatesUnderMouse(out tileX, out tileY))
                tileMapCircle.SetTile(tileX, tileY, 0);
        }
        else if (Input.GetMouseButton(1))
        {
            int tileX, tileY;
            if (GetTileCoordinatesUnderMouse(out tileX, out tileY))
                tileMapCircle.SetTile(tileX, tileY, 1);
        }

        float zoom = Input.GetAxis("Mouse ScrollWheel");
        cameraDistance -= cameraDistance * zoom * zoomSpeed * Time.deltaTime;
        cameraDistance = Mathf.Clamp(cameraDistance, 3, 100);

        velocity.x = Input.GetAxis("Horizontal") * walkSpeed;
        if ((hitFlags & TileHitFlags.Down) != 0 && (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space)))
            velocity.y = jumpSpeed;

        UpdatePosition();

        UpdateCamera();
	}

    private bool GetTileCoordinatesUnderMouse(out int tileX, out int tileY)
    {
        Camera cam = Camera.main;

        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

        return tileMapCircle.GetTileCoordinatesFromPosition(worldPos, out tileX, out tileY);
    }

    private void UpdateCamera()
    {
        Camera cam = Camera.main;

        cam.transform.position = transform.position - Vector3.forward * 10.0f;
        cam.transform.rotation = Quaternion.AngleAxis(-tileMapCircle.GetAngleFromPosition(transform.position), Vector3.forward);
        cam.orthographicSize = cameraDistance * tileMapCircle.GetScaleFromPosition(transform.position);
    }
}

