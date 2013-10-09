using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TilemapCircle : MonoBehaviour 
{
    public const float TILE_SIZE = 0.5f; 
    public const float TILE_SIZE_INV = 1.0f / TILE_SIZE;

    public bool debugColor;
    public Material material;

    public int seed;
    public int height = 10;
    public int width;

    private TilemapSegmentRender[] renderers;

    private byte[] tiles;

    private Color32[] colorsPerTile;

    private Vector3[] circlePoints;
    private float[] circleHeights;

    private int lastHeight;
    private int lastWidth;

	// Use this for initialization
	void Start () 
    {
        UpdateData();

        UpdateRenderers();

        UpdateMesh();
	}
	
	void Update () 
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            UpdateData();

            UpdateRenderers();
        }

        UpdateMesh();
	}

    private void UpdateMesh()
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].UpdateMesh();
    }

    private void UpdateRenderers()
    {
        int renderersAmount = Mathf.Clamp(Mathf.CeilToInt((width * height) / (32 * 32)), 1, 256);

        bool recreate = false;

        if (Application.isEditor && !Application.isPlaying)
        {
            if (renderers != null)
            {
                foreach (TilemapSegmentRender rend in renderers)
                    if (!rend)
                        recreate = true;
            }
        }

        if (renderers == null || renderers.Length != renderersAmount || recreate || lastWidth != width || lastHeight != height)
        {
            if (renderers != null)
            {
                foreach (TilemapSegmentRender rend in renderers)
                {
                    if (rend)
                    {
                        if (Application.isPlaying)
                            GameObject.Destroy(rend.gameObject);
                        else
                            GameObject.DestroyImmediate(rend.gameObject);
                    }
                }
            }

            renderers = new TilemapSegmentRender[renderersAmount];

            lastWidth = width;
            lastHeight = height;

            for (int i = 0; i < renderers.Length; i++)
            {
                GameObject go = new GameObject("Renderer " + i);
                go.hideFlags = HideFlags.DontSave;
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                go.AddComponent<MeshRenderer>().sharedMaterial = material;

                renderers[i] = go.AddComponent<TilemapSegmentRender>();

                renderers[i].Init(
                    this, 
                    (width * i) / renderers.Length,
                    Mathf.Min((width * (i + 1)) / renderers.Length, width),
                    circlePoints,
                    circleHeights,
                    colorsPerTile);
            }
        }

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].SetDirty();
    }

    private void UpdateData()
    {
        if (height < 5)
            height = 5;

        width = (((int)((float)height * Mathf.PI * 2.0f)) / 4) * 4;

        if (circlePoints == null || circlePoints.Length != width)
            circlePoints = new Vector3[width];

        if (circleHeights == null || circleHeights.Length != height + 1)
            circleHeights = new float[height + 1];

        if (tiles == null || tiles.Length != width * height)
            tiles = new byte[width * height];

        Random.seed = seed;
        for (int i = 0; i < tiles.Length; i++)
        {
            //if (Random.value > 0.85f)
            //    tiles[i] = 0;
            //else
                tiles[i] = (byte)Random.Range(1, 256);
        }

        if (colorsPerTile == null || colorsPerTile.Length != 256)
        {
            colorsPerTile = new Color32[256];
            for (int i = 0; i < colorsPerTile.Length; i++)
            {
                colorsPerTile[i] = Color.white;

                colorsPerTile[i] = new Color32(
                    (byte) Random.Range(127, 255),
                    (byte) Random.Range(127, 255),
                    (byte) Random.Range(127, 255), 
                    255);
            }
        }

        float angleStep = ((2.0f * Mathf.PI) / width);

        for (int i = 0; i < width; i++)
        {
            float angle = i * angleStep;
            circlePoints[i].x = Mathf.Sin(angle);
            circlePoints[i].y = Mathf.Cos(angle);
        }

        circleHeights[0] = (height - 1) * TILE_SIZE;

        float k = -((width / (Mathf.PI * 2.0f))) / (1 - (width / (Mathf.PI * 2.0f)));

        for (int i = 1; i <= height; i++)
        {
            float r1 = circleHeights[i - 1];
            //float r2 = ((-r1 * width) / (Mathf.PI * 2.0f)) / (1 - (width / (Mathf.PI * 2.0f)));

            float r2 = r1 * k;

            circleHeights[i] = r2;
        }

        //Debug.Log(circleHeights[0] + " / " + circleHeights[circleHeights.Length - 1]);
    }

    public byte GetTile(int tileX, int tileY)
    {
        return tiles[tileX + tileY * width];
    }

    public void SetTile(int tileX, int tileY, byte tile)
    {
        if (tiles[tileX + tileY * width] != tile)
        {
            tiles[tileX + tileY * width] = tile;

            if (renderers != null && renderers.Length > 0)
            {
                int rendererIndex = (tileX * renderers.Length) / width;

                renderers[rendererIndex].SetDirty();
            }

            //dirty = true;
        }
    }

    /*
    public void OnDrawGizmos()
    {
        Gizmos.DrawSphere(GetTileFloorPoint(0, 0), 0.5f);

        Gizmos.DrawSphere(GetTileFloorPoint(width - 1, height - 1), 0.5f);
    }

    public void OnDrawGizmosSelected()
    {
        OnDrawGizmos();
    }
    */

    /// <summary>
    /// Returns the coordinates of the tile closest to the given position
    /// </summary>
    public bool GetTileCoordinatesFromPosition(Vector3 position, out int tileX, out int tileY)
    {
        float dx = position.x - transform.position.x;
        float dy = position.y - transform.position.y;

        float distance = Mathf.Sqrt(dx * dx + dy * dy);

        if (distance >= circleHeights[circleHeights.Length - 1])
        {
            tileX = 0;
            tileY = 0;

            return false;
        }

        float angle = -Mathf.Atan2(dy, dx) + Mathf.PI * 0.5f;

        while (angle > Mathf.PI * 2.0f)
            angle -= Mathf.PI * 2.0f;

        while (angle < 0.0f)
            angle += Mathf.PI * 2.0f;

        tileX = (int) ((angle / (Mathf.PI * 2.0f)) * width);
        tileX = tileX % width;

        tileY = height - 1;
        for (int i = 1; i < circleHeights.Length; i++)
        {
            if (distance < circleHeights[i])
            {
                tileY = i - 1;
                break;
            }
        }

        return true;
    }

    public float GetScaleFromPosition(Vector3 position)
    {
        float dx = position.x - transform.position.x;
        float dy = position.y - transform.position.y;
        float distance = Mathf.Sqrt(dx * dx + dy * dy);

        float scale = Mathf.Clamp(
            (distance * 2.0f * Mathf.PI) / width,
            (circleHeights[0] * 2.0f * Mathf.PI) / width,
            (circleHeights[circleHeights.Length - 1] * 2.0f * Mathf.PI) / width) * TILE_SIZE_INV;

        return scale;
    }

    public Vector3 GetNormalFromPosition(Vector3 position)
    {
        float dx = position.x - transform.position.x;
        float dy = position.y - transform.position.y;
        float distance = Mathf.Sqrt(dx * dx + dy * dy);

        return new Vector3(dx / distance, dy / distance, 0.0f);
    }

    public Vector3 GetTangentFromPosition(Vector3 position)
    {
        Vector3 normal = GetNormalFromPosition(position);

        return new Vector3(normal.y, -normal.x, 0.0f);
    }

    public TileHitInfo GetHitInfo(Vector3 position)
    {
        TileHitInfo hitInfo = new TileHitInfo();

        float dx = position.x - transform.position.x;
        float dy = position.y - transform.position.y;
        hitInfo.originMapDistance = Mathf.Sqrt(dx * dx + dy * dy);

        if (hitInfo.originMapDistance < 0.00001f)
            hitInfo.originMapDistance = 0.00001f;

        hitInfo.originMapAngle = -Mathf.Atan2(dy, dx) + Mathf.PI * 0.5f;

        while (hitInfo.originMapAngle > Mathf.PI * 2.0f)
            hitInfo.originMapAngle -= Mathf.PI * 2.0f;

        while (hitInfo.originMapAngle < 0.0f)
            hitInfo.originMapAngle += Mathf.PI * 2.0f;

        hitInfo.hitTileX = (int) ((hitInfo.originMapAngle / (Mathf.PI * 2.0f)) * width);
        hitInfo.hitTileX = hitInfo.hitTileX % width;

        hitInfo.hitTileY = height - 1;
        for (int i = 1; i < circleHeights.Length; i++)
        {
            if (hitInfo.originMapDistance < circleHeights[i])
            {
                hitInfo.hitTileY = i - 1;
                break;
            }
        }

        while (hitInfo.hitTileY > 0 && GetTile(hitInfo.hitTileX, hitInfo.hitTileY) == 0)
            hitInfo.hitTileY--;

        hitInfo.hitDistance = hitInfo.originMapDistance - circleHeights[hitInfo.hitTileY + 1];

        hitInfo.originNormal = new Vector3(dx / hitInfo.originMapDistance, dy / hitInfo.originMapDistance, 0.0f);
        hitInfo.originTangent = new Vector3(hitInfo.originNormal.y, -hitInfo.originNormal.x, 0.0f);

        hitInfo.scale = Mathf.Clamp(
            (hitInfo.originMapDistance * 2.0f * Mathf.PI) / width,
            (circleHeights[0] * 2.0f * Mathf.PI) / width,
            (circleHeights[circleHeights.Length - 1] * 2.0f * Mathf.PI) / width) * TILE_SIZE_INV;

        //Debug.Log(hitInfo.distance + " / " + hitInfo.floorDistance);

        return hitInfo;
    }

    public bool Raycast(Vector3 origin, Vector3 direction, float len, out TileHitInfo hitInfo)
    {
        hitInfo = new TileHitInfo();

        float dx = origin.x - transform.position.x;
        float dy = origin.y - transform.position.y;

        hitInfo.originMapDistance = Mathf.Sqrt(dx * dx + dy * dy);

        if (hitInfo.originMapDistance < 0.00001f)
            hitInfo.originMapDistance = 0.00001f;

        hitInfo.originMapAngle = -Mathf.Atan2(dy, dx) + Mathf.PI * 0.5f;

        while (hitInfo.originMapAngle > Mathf.PI * 2.0f)
            hitInfo.originMapAngle -= Mathf.PI * 2.0f;

        while (hitInfo.originMapAngle < 0.0f)
            hitInfo.originMapAngle += Mathf.PI * 2.0f;

        hitInfo.originNormal = new Vector3(dx / hitInfo.originMapDistance, dy / hitInfo.originMapDistance, 0.0f);
        hitInfo.originTangent = new Vector3(hitInfo.originNormal.y, -hitInfo.originNormal.x, 0.0f);

        hitInfo.scale = Mathf.Clamp(
            (hitInfo.originMapDistance * 2.0f * Mathf.PI) / width,
            (circleHeights[0] * 2.0f * Mathf.PI) / width,
            (circleHeights[circleHeights.Length - 1] * 2.0f * Mathf.PI) / width) * TILE_SIZE_INV;

        if (direction == Vector3.right)
        {

        }
        else if (direction == Vector3.left)
        {

        }
        else if (direction == Vector3.up)
        {

        }
        else if (direction == Vector3.down)
        {
            hitInfo.hitTileX = (int) ((hitInfo.originMapAngle / (Mathf.PI * 2.0f)) * width);
            hitInfo.hitTileX = hitInfo.hitTileX % width;

            hitInfo.hitTileY = height - 1;
            for (int i = 1; i < circleHeights.Length; i++)
            {
                if (hitInfo.originMapDistance < circleHeights[i])
                {
                    hitInfo.hitTileY = i - 1;
                    break;
                }
            }

            while (hitInfo.hitTileY > 0 && GetTile(hitInfo.hitTileX, hitInfo.hitTileY) == 0)
                hitInfo.hitTileY--;

            hitInfo.hitDistance = hitInfo.originMapDistance - circleHeights[hitInfo.hitTileY + 1];
        }

        //Debug.Log(hitInfo.distance + " / " + hitInfo.floorDistance);

        return true;
    }
    /// <summary>
    /// Returns the floor position closest to the given position
    /// </summary>
    public Vector3 GetFloorPositionFromPosition(Vector3 position)
    {
        TileHitInfo hitInfo = GetHitInfo(position);

        return position - hitInfo.originNormal * hitInfo.hitDistance;
    }

}
