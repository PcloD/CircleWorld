using UnityEngine;
using System.Collections.Generic;

public class TilemapSegmentRender : MonoBehaviour
{
    public Mesh mesh;

    private bool dirty;

    private int fromX;
    private int toX;
    private TilemapCircle map;
    private Vector3[] circleNormals;
    private float[] circleHeights;
    private Color32[] colorsPerTile;

    private Vector3[] vertices;
    private Color32[] colors;
    private int[] triangles;
    private Vector2[] uvs;

    private bool firstTime;

    public void Init(TilemapCircle map, int fromX, int toX, Vector3[] circleNormals, float[] circleHeights, Color32[] colorsPerTile)
    {
        dirty = true;
        firstTime = true;

        this.map = map;
        this.fromX = fromX;
        this.toX = toX;
        this.circleNormals = circleNormals;
        this.circleHeights = circleHeights;
        this.colorsPerTile = colorsPerTile;

        if (!GetComponent<MeshRenderer>())
            gameObject.AddComponent<MeshRenderer>();

        if (!GetComponent<MeshFilter>())
            gameObject.AddComponent<MeshFilter>();

        if (mesh == null) 
            mesh = new Mesh();

        int vertexCount = (toX - fromX) * map.height * 4;
        int triangleCount = (toX - fromX) * map.height * 6;

        if (vertices == null || vertices.Length != vertexCount)
            vertices = new Vector3[vertexCount];

        if (colors == null || colors.Length != vertexCount)
            colors = new Color32[vertexCount];

        if (uvs == null || uvs.Length != vertexCount)
            uvs = new Vector2[vertexCount];

        if (triangles == null || triangles.Length != triangleCount)
            triangles = new int[triangleCount];
    }

    public void SetDirty()
    {
        dirty = true;
    }

    public void UpdateMesh()
    {
        if (!dirty)
            return;

        dirty = false;

        int vertexOffset = 0;
        int triangleOffset = 0;

        Vector3 p1, p2, p3, p4;

        float tx = 1.0f / 16.0f;
        float ty = 1.0f / 16.0f;
        float tt = 1.0f / 256.0f;

        for (int y = 0; y < map.height; y++)
        {
            float upRadius = circleHeights[y + 1];
            float downRadius = circleHeights[y];

            for (int x = fromX; x < toX; x++)
            {
                byte tile = map.GetTile(x, y);

                if (tile == 0) //skip empty tiles
                {
                    p1 = p2 = p3 = p4 = Vector3.zero;
                }
                else
                {
                    p1 = circleNormals[x] * upRadius;
                    p2 = circleNormals[(x + 1) % map.width] * upRadius;
                    p3 = circleNormals[(x + 1) % map.width] * downRadius;
                    p4 = circleNormals[x] * downRadius;
                }

                vertices[vertexOffset + 0] = p1;
                vertices[vertexOffset + 1] = p2;
                vertices[vertexOffset + 2] = p3;
                vertices[vertexOffset + 3] = p4;

                int textureId = tile % 4;

                if (map.debugColor)
                {
                    colors[vertexOffset + 0] = Color.red;
                    colors[vertexOffset + 1] = Color.green;
                    colors[vertexOffset + 2] = Color.blue;
                    colors[vertexOffset + 3] = Color.cyan;
                }
                else
                {
                    Color32 color = colorsPerTile[tile];

                    colors[vertexOffset + 0] = color;
                    colors[vertexOffset + 1] = color;
                    colors[vertexOffset + 2] = color;
                    colors[vertexOffset + 3] = color;
                }

                uvs[vertexOffset + 0] = new Vector2(tx * textureId, 1.0f - 0);
                uvs[vertexOffset + 1] = new Vector2(tx * textureId + tx - tt, 1.0f - 0);
                uvs[vertexOffset + 2] = new Vector2(tx * textureId + tx - tt, 1.0f - (0 + ty) + tt);
                uvs[vertexOffset + 3] = new Vector2(tx * textureId, 1.0f - (0 + ty) + tt);

                if (firstTime)
                {
                    triangles[triangleOffset + 0] = vertexOffset + 0;
                    triangles[triangleOffset + 1] = vertexOffset + 1;
                    triangles[triangleOffset + 2] = vertexOffset + 2;

                    triangles[triangleOffset + 3] = vertexOffset + 2;
                    triangles[triangleOffset + 4] = vertexOffset + 3;
                    triangles[triangleOffset + 5] = vertexOffset + 0;
                }

                vertexOffset += 4;
                triangleOffset += 6;
            }
        }

        mesh.vertices = vertices;

        if (firstTime)
        {
            mesh.triangles = triangles;
            mesh.colors32 = colors;
            mesh.uv = uvs;
        }

        GetComponent<MeshFilter>().sharedMesh = mesh;

        firstTime = false;
    }

}


