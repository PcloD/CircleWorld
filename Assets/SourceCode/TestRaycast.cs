using UnityEngine;
using UniverseEngine;

public class TestRaycast : MonoBehaviour 
{
    public TilemapCircleView map;
    public TileDirection direction;
    public float len = 100.0f;
    public float radius = 0.25f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDrawGizmosSelected()
    {
        OnDrawGizmos();
    }

    void OnDrawGizmos()
    {
        if (map)
        {
            //int tileX, tileY;
            //map.GetTileCoordinatesFromPosition(transform.position, out tileX, out tileY);
            //Debug.Log(tileX + " / " + tileY);

            TileHitInfo hitInfo;

            Vector2 origin = Vector3.zero;
            Vector2 target = Vector3.zero;

            if (direction == TileDirection.Down)
            {
                origin = transform.position + (Vector3) (map.TilemapCircle.GetNormalFromPosition(transform.position) * radius);
                target = transform.position - (Vector3) (map.TilemapCircle.GetNormalFromPosition(transform.position) * (len - radius));
            }
            else if (direction == TileDirection.Up)
            {
                origin = transform.position - (Vector3) (map.TilemapCircle.GetNormalFromPosition(transform.position) * radius);
                target = transform.position + (Vector3) (map.TilemapCircle.GetNormalFromPosition(transform.position) * (len - radius));
            }
            else if (direction == TileDirection.Right)
            {
                origin = transform.position - (Vector3) (map.TilemapCircle.GetTangentFromPosition(transform.position) * radius);
                target = transform.position + (Vector3) (map.TilemapCircle.GetTangentFromPosition(transform.position) * (len - radius));
            }
            else if (direction == TileDirection.Left)
            {
                origin = transform.position + (Vector3) (map.TilemapCircle.GetTangentFromPosition(transform.position) * radius);
                target = transform.position - (Vector3) (map.TilemapCircle.GetTangentFromPosition(transform.position) * (len - radius));
            }

            if (map.TilemapCircle.RaycastSquare(origin, radius * 2.0f, direction, len, out hitInfo))
            {
                //Debug.Log(string.Format("hit tileX: {0} tileY: {1} hitDistance: {2}", hitInfo.hitTileX, hitInfo.hitTileY, hitInfo.hitDistance));
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(transform.position, radius);

                Vector2 hitPosition = origin - hitInfo.hitNormal * hitInfo.hitDistance;

                Gizmos.color = Color.red;
                Gizmos.DrawLine(hitPosition, target);

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(origin, hitPosition);
            }
            else
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(transform.position, radius);
                //Debug.Log("no hit");
            }
        }
    }
}
