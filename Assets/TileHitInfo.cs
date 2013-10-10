using UnityEngine;

public struct TileHitInfo
{
    public float originMapDistance;
    public float originMapAngle;

    public Vector3 originNormal;
    public Vector3 originTangent;

    public int hitTileX;
    public int hitTileY;

    public float hitDistance;
    public Vector3 hitNormal;
    public Vector3 hitPosition;

    public float scale;
}

