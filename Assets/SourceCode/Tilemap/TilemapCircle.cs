using UnityEngine;
using System.Collections.Generic;

//[ExecuteInEditMode]
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

    private Vector3[] circleNormals;
    private float[] circleHeights;

    private int lastHeight;
    private int lastWidth;

    //Used when fiding tileY positions!
    private float height0;
    private float k;
    private float logk;

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

            int sizeX = Mathf.CeilToInt((float) width / (float) renderers.Length);

            int fromX = 0;
            int toX = sizeX;

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
                    fromX,
                    toX,
                    circleNormals,
                    circleHeights,
                    colorsPerTile);

                fromX += sizeX;
                toX += sizeX;

                if (toX >= width)
                    toX = width;
            }
        }

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].SetDirty();
    }

    private TilemapSegmentRender GetRenderer(int tileX, int tileY)
    {
        if (renderers != null && renderers.Length > 0)
        {
            int sizeX = Mathf.CeilToInt((float) width / (float) renderers.Length);

            int rendererIndex = tileX / sizeX;

            return renderers[rendererIndex];
        }

        return null;
    }

    private void UpdateData()
    {
        if (height < 5)
            height = 5;

        width = (((int)((float)height * Mathf.PI * 2.0f)) / 4) * 4;

        if (circleNormals == null || circleNormals.Length != width)
            circleNormals = new Vector3[width];

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
            circleNormals[i].x = Mathf.Sin(angle);
            circleNormals[i].y = Mathf.Cos(angle);
        }

        height0 = (height - 1) * TILE_SIZE;
        k = -((width / (Mathf.PI * 2.0f))) / (1 - (width / (Mathf.PI * 2.0f)));
        logk = Mathf.Log(k);

        circleHeights[0] = height0;

        for (int i = 1; i <= height; i++)
        {
            float r1 = circleHeights[i - 1];

            //float r2 = ((-r1 * width) / (Mathf.PI * 2.0f)) / (1 - (width / (Mathf.PI * 2.0f)));
            float r2 = r1 * k;

            circleHeights[i] = r2;
        }
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

            TilemapSegmentRender renderer = GetRenderer(tileX, tileY);

            if (renderer != null)
                renderer.SetDirty();
        }
    }

    public int GetTileYFromDistance(float distance)
    {
        //This was taken from wolfram-alpha, by solving the radius relationship function
        //Original function: http://www.wolframalpha.com/input/?i=g%280%29%3Dk%2C+g%28n%2B1%29%3Dl+*+g%28n%29
        //Solution: http://www.wolframalpha.com/input/?i=y+%3D+k+*+l%CB%86x+find+x (we use the solution over reals with y > 0)

        //int tileY = (int) (Mathf.Log (distance / height0) / Mathf.Log (k));
        int tileY = (int) (Mathf.Log (distance / height0) / logk);

        return tileY;
    }

    public float GetDistanceFromTileY(int tileY)
    {
        return height0 * Mathf.Pow(k, (float) tileY);
    }

    public int GetTileXFromAngle(float angle)
    {
        int tileX = Mathf.FloorToInt((angle / (Mathf.PI * 2.0f)) * width);

        tileX = tileX % width;
        if (tileX < 0)
            tileX += width;

        return tileX;
    }

    public Vector3 GetPositionFromTileCoordinate(int tileX, int tileY)
    {
        return transform.position + GetNormalFromTileX(tileX) * GetDistanceFromTileY(tileY);
    }

    public bool GetTileCoordinatesFromPosition(Vector3 position, out int tileX, out int tileY)
    {
        float dx = position.x - transform.position.x;
        float dy = position.y - transform.position.y;

        float distance = Mathf.Sqrt(dx * dx + dy * dy);
        float angle = -Mathf.Atan2(dy, dx) + Mathf.PI * 0.5f;

        tileY = GetTileYFromDistance(distance);
        tileX = GetTileXFromAngle(angle);

        if (tileY >= height || tileY < 0)
            return false;

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

    public float GetAngleFromPosition(Vector3 position)
    {
        float dx = position.x - transform.position.x;
        float dy = position.y - transform.position.y;

        float angle = -Mathf.Atan2(dy, dx) + Mathf.PI * 0.5f;

        return angle * Mathf.Rad2Deg;
    }
    public Vector3 GetNormalFromTileX(int tileX)
    {
        tileX = tileX % width;;

        return circleNormals[tileX];
    }

    public Vector3 GetTangentFromPosition(Vector3 position)
    {
        Vector3 normal = GetNormalFromPosition(position);

        return new Vector3(normal.y, -normal.x, 0.0f);
    }

    public Vector3 GetTangentFromTileCoordinate(int tileX, int tileY)
    {
        Vector3 normal = GetNormalFromTileX(tileX);

        return new Vector3(normal.y, -normal.x, 0.0f);
    }

    public bool RaycastSquare(Vector3 origin, float size, TileDirection direction, float len, out TileHitInfo hitInfo)
    {
        size *= 0.95f;

        hitInfo = new TileHitInfo();

        int iterations = Mathf.Max(Mathf.CeilToInt(size / TILE_SIZE), 1);

        Vector3 from = origin - GetTanget(origin, direction) * (size * 0.5f);
        Vector3 step = GetTanget(origin, direction) * (size / iterations);

        bool hitAny = false;
        TileHitInfo localHitInfo;

        for (int i = 0; i <= iterations; i++)
        {
            if (Raycast(from, direction, len, out localHitInfo))
            {
                if (!hitAny)
                {
                    hitAny = true;
                    hitInfo = localHitInfo;
                }
                else if (localHitInfo.hitDistance < hitInfo.hitDistance)
                {
                    hitInfo = localHitInfo;
                }
            }

            from += step;
        }

        return hitAny;
    }

    public Vector3 GetDirection(Vector3 origin, TileDirection direction)
    {
        switch(direction)
        {
            case TileDirection.Down:
                return -GetNormalFromPosition(origin);
            
            case TileDirection.Up:
                return GetNormalFromPosition(origin);

            case TileDirection.Right:
                return GetTangentFromPosition(origin);

            case TileDirection.Left:
                return -GetTangentFromPosition(origin);
        }

        return Vector3.zero;
    }

    
    public Vector3 GetTanget(Vector3 origin, TileDirection direction)
    {
        switch(direction)
        {
            case TileDirection.Down:
                return GetTangentFromPosition(origin);

            case TileDirection.Up:
                return GetTangentFromPosition(origin);

            case TileDirection.Right:
                return GetNormalFromPosition(origin);

            case TileDirection.Left:
                return GetNormalFromPosition(origin);
        }

        return Vector3.zero;
    }
    public bool Raycast(Vector3 origin, TileDirection direction, float len, out TileHitInfo hitInfo)
    {
        hitInfo = new TileHitInfo();

        float dx = origin.x - transform.position.x;
        float dy = origin.y - transform.position.y;
        float originDistance = Mathf.Sqrt(dx * dx + dy * dy);

        Vector3 target;
        float targetdx;
        float targetdy;
        float targetDistance;
        float tangentDistance;

        float segmentSize;

        if (originDistance < 0.001f)
            originDistance = 0.001f;

        float originMapAngle = -Mathf.Atan2(dy, dx) + Mathf.PI * 0.5f;

        while (originMapAngle > Mathf.PI * 2.0f)
            originMapAngle -= Mathf.PI * 2.0f;

        while (originMapAngle < 0.0f)
            originMapAngle += Mathf.PI * 2.0f;

        Vector3 originNormal = new Vector3(dx / originDistance, dy / originDistance, 0.0f);
        Vector3 originTangent = new Vector3(originNormal.y, -originNormal.x, 0.0f);

        if (direction == TileDirection.Right)
        {
            target = origin + originTangent * len;
            targetdx = target.x - transform.position.x;
            targetdy = target.y - transform.position.y;
            targetDistance = Mathf.Sqrt(targetdx * targetdx + targetdy * targetdy);

            if (originDistance > circleHeights[circleHeights.Length - 1])
            {
                //Origin point outside, not hit!
                return false;
            }

            for (int i = 1; i < circleHeights.Length; i++)
            {
                if (originDistance < circleHeights[i])
                {
                    hitInfo.hitTileY = i - 1;
                    break;
                }
            }

            segmentSize = (circleHeights[hitInfo.hitTileY] * 2.0f * Mathf.PI) / width;
            tangentDistance = ((originMapAngle / (Mathf.PI * 2.0f)) * width);

            hitInfo.hitTileX = (int)tangentDistance;
            hitInfo.hitTileX = (hitInfo.hitTileX + 1) % width;

            len -= segmentSize * (Mathf.Ceil(tangentDistance) - tangentDistance);

            while (hitInfo.hitTileX < width && len >= 0)
            {
                if (GetTile(hitInfo.hitTileX, hitInfo.hitTileY) != 0)
                {
                    hitInfo.hitNormal = -GetTangentFromTileCoordinate(hitInfo.hitTileX, hitInfo.hitTileY);

                    hitInfo.hitPosition = transform.position + 
                        circleNormals[hitInfo.hitTileX] * originDistance;

                    hitInfo.hitDistance = (origin - hitInfo.hitPosition).magnitude;
                    return true;
                }

                len -= segmentSize;

                hitInfo.hitTileX++;
            }
        }
        else if (direction == TileDirection.Left)
        {
            target = origin + originTangent * len;
            targetdx = target.x - transform.position.x;
            targetdy = target.y - transform.position.y;
            targetDistance = Mathf.Sqrt(targetdx * targetdx + targetdy * targetdy);

            if (originDistance > circleHeights[circleHeights.Length - 1])
            {
                //Origin point outside, not hit!
                return false;
            }

            for (int i = 1; i < circleHeights.Length; i++)
            {
                if (originDistance < circleHeights[i])
                {
                    hitInfo.hitTileY = i - 1;
                    break;
                }
            }

            segmentSize = (circleHeights[hitInfo.hitTileY] * 2.0f * Mathf.PI) / width;
            tangentDistance = ((originMapAngle / (Mathf.PI * 2.0f)) * width);

            hitInfo.hitTileX = (int)tangentDistance;
            hitInfo.hitTileX = (hitInfo.hitTileX - 1) % width;
            if (hitInfo.hitTileX < 0)
                hitInfo.hitTileX += width;

            len -= segmentSize * (tangentDistance - Mathf.Floor(tangentDistance));

            while (hitInfo.hitTileX >= 0 && len >= 0)
            {
                if (GetTile(hitInfo.hitTileX, hitInfo.hitTileY) != 0)
                {
                    hitInfo.hitNormal = GetTangentFromTileCoordinate(hitInfo.hitTileX + 1, hitInfo.hitTileY);

                    hitInfo.hitPosition = transform.position + 
                        circleNormals[(hitInfo.hitTileX + 1) % width] * originDistance;

                    hitInfo.hitDistance = (origin - hitInfo.hitPosition).magnitude;
                    return true;
                }

                len -= segmentSize;

                hitInfo.hitTileX--;
            }
        }
        else if (direction == TileDirection.Up)
        {
            target = origin + originNormal * len;
            targetdx = target.x - transform.position.x;
            targetdy = target.y - transform.position.y;
            targetDistance = Mathf.Sqrt(targetdx * targetdx + targetdy * targetdy);

            if (originDistance > circleHeights[circleHeights.Length - 1])
            {
                //Origin point outside, not hit!
                return false;
            }

            hitInfo.hitTileX = (int) ((originMapAngle / (Mathf.PI * 2.0f)) * width);
            hitInfo.hitTileX = hitInfo.hitTileX % width;

            for (int i = 1; i < circleHeights.Length; i++)
            {
                if (originDistance < circleHeights[i])
                {
                    hitInfo.hitTileY = i;
                    len -= circleHeights[i] - originDistance;
                    break;
                }
            }

            while (hitInfo.hitTileY < height && len >= 0)
            {
                if (GetTile(hitInfo.hitTileX, hitInfo.hitTileY) != 0)
                {
                    hitInfo.hitNormal = -originNormal;
                    hitInfo.hitPosition = transform.position + originNormal * circleHeights[hitInfo.hitTileY];
                    hitInfo.hitDistance = (origin - hitInfo.hitPosition).magnitude;
                    return true;
                }

                if (hitInfo.hitTileY < height - 1 )
                    len -= (circleHeights[hitInfo.hitTileY + 1] - circleHeights[hitInfo.hitTileY]);

                hitInfo.hitTileY++;
            }
        }
        else if (direction == TileDirection.Down)
        {
            target = origin - originNormal * len;
            targetdx = target.x - transform.position.x;
            targetdy = target.y - transform.position.y;
            targetDistance = Mathf.Sqrt(targetdx * targetdx + targetdy * targetdy);

            if (/*originDistance > circleHeights[circleHeights.Length - 1] &&*/
                targetDistance > circleHeights[circleHeights.Length - 1])
            {
                //Target outside, no hit!
                return false;
            }

            hitInfo.hitTileX = (int) ((originMapAngle / (Mathf.PI * 2.0f)) * width);
            hitInfo.hitTileX = hitInfo.hitTileX % width;

            for (int i = circleHeights.Length - 1; i >= 1; i--)
            {
                if (originDistance > circleHeights[i])
                {
                    hitInfo.hitTileY = i - 1;
                    len -= originDistance - circleHeights[i];
                    break;
                }
            }

            while (hitInfo.hitTileY >= 0 && len > 0)
            {
                if (GetTile(hitInfo.hitTileX, hitInfo.hitTileY) != 0)
                {
                    hitInfo.hitNormal = originNormal;
                    hitInfo.hitPosition = transform.position + originNormal * circleHeights[hitInfo.hitTileY + 1];
                    hitInfo.hitDistance = (origin - hitInfo.hitPosition).magnitude;
                    return true;
                }

                if (hitInfo.hitTileY > 0)
                    len -= (circleHeights[hitInfo.hitTileY] - circleHeights[hitInfo.hitTileY - 1]);
                hitInfo.hitTileY--;
            }

            if (hitInfo.hitTileY < 0 && len >= circleHeights[1] - circleHeights[0])
            {
                //Core hit!
                hitInfo.hitTileY = 0;
                hitInfo.hitNormal = originNormal;
                hitInfo.hitPosition = transform.position + originNormal * circleHeights[hitInfo.hitTileY];
                hitInfo.hitDistance = (origin - hitInfo.hitPosition).magnitude;
                return true;
            }
        }

        return false;
    }
}
