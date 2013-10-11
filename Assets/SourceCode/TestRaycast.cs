using UnityEngine;
using System.Collections;

public class TestRaycast : MonoBehaviour 
{
    public TilemapCircle map;
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

            Vector3 origin = Vector3.zero;
            Vector3 target = Vector3.zero;

            if (direction == TileDirection.Down)
            {
                origin = transform.position + map.GetNormalFromPosition(transform.position) * radius;
                target = transform.position - map.GetNormalFromPosition(transform.position) * (len - radius);
            }
            else if (direction == TileDirection.Up)
            {
                origin = transform.position - map.GetNormalFromPosition(transform.position) * radius;
                target = transform.position + map.GetNormalFromPosition(transform.position) * (len - radius);
            }
            else if (direction == TileDirection.Right)
            {
                origin = transform.position - map.GetTangentFromPosition(transform.position) * radius;
                target = transform.position + map.GetTangentFromPosition(transform.position) * (len - radius);
            }
            else if (direction == TileDirection.Left)
            {
                origin = transform.position + map.GetTangentFromPosition(transform.position) * radius;
                target = transform.position - map.GetTangentFromPosition(transform.position) * (len - radius);
            }

            if (map.RaycastSquare(origin, radius * 2.0f, direction, len, out hitInfo))
            {
                //Debug.Log(string.Format("hit tileX: {0} tileY: {1} hitDistance: {2}", hitInfo.hitTileX, hitInfo.hitTileY, hitInfo.hitDistance));
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(transform.position, radius);

                Vector3 hitPosition = origin - hitInfo.hitNormal * hitInfo.hitDistance;

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
