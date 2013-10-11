using UnityEngine;
using System.Collections;

public class Avatar : TilemapObject 
{
    public float jumpSpeed = 7.0f;
    public float walkSpeed = 3.0f;

	// Use this for initialization
	
    public override void Start()
    {
        base.Start();

        //Snap to floor on start
        TileHitInfo hitInfo = tileMapCircle.GetHitInfo(transform.position);

        transform.position -= hitInfo.originNormal * (hitInfo.hitDistance - hitInfo.scale);
        transform.up = hitInfo.originNormal;
	}
	
    public override void Update()
    {
        if (Input.GetMouseButton(0))
        {
            int tileX, tileY;
            if (GetTileCoordinatesUnderMouse(out tileX, out tileY))
                tileMapCircle.SetTile(tileX, tileY, 0);
        }

        velocity.x = Input.GetAxis("Horizontal") * walkSpeed;
        if (onFloor && (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space)))
        {
            Debug.Log("jump!");
            velocity.y = jumpSpeed;
        }

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
        cam.transform.up = tileMapCircle.GetNormalFromPosition(transform.position);
        cam.orthographicSize = 10 * tileMapCircle.GetScaleFromPosition(transform.position);
    }
}

