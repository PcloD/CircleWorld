using UnityEngine;
using UniverseEngine;
using System.Collections.Generic;

public class TilemapCircleViewRenderer : MonoBehaviour
{
    public Mesh mesh;

    private bool dirty;

    private int fromX;
    private int toX;
    private TilemapCircleView tilemapCircleView;
    private TilemapCircle tilemapCircle;
    
    private Vector2[] circleNormals;
    private float[] circleHeights;

    private bool firstTime;
    
    private MeshFilter meshFilter;
    
    static private TileType[] tileTypes;
    
    public void Init(TilemapCircleView tilemapCircleView, int fromX, int toX)
    {
        if (tileTypes == null)
            tileTypes = TileTypes.GetTileTypes();
        
        dirty = true;
        firstTime = true;

        this.tilemapCircleView = tilemapCircleView;
        this.tilemapCircle = tilemapCircleView.TilemapCircle;
        this.fromX = fromX;
        this.toX = toX;
        this.circleNormals = tilemapCircle.CircleNormals;
        this.circleHeights = tilemapCircle.CircleHeights;

        if (!GetComponent<MeshRenderer>())
            gameObject.AddComponent<MeshRenderer>();

        if (!GetComponent<MeshFilter>())
            gameObject.AddComponent<MeshFilter>();

        if (mesh == null) 
            mesh = new Mesh();
        
        meshFilter = GetComponent<MeshFilter>();
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
          
        int height = tilemapCircleView.TilemapCircle.Height;
        int width = tilemapCircleView.TilemapCircle.Width;
        
        int vertexCount = (toX - fromX) * tilemapCircle.Height * 4;
        int triangleCount = (toX - fromX) * tilemapCircle.Height * 6;
        
        Vector3[] vertices = DataPools.poolVector3.GetArray(vertexCount);
        Color32[] colors = DataPools.poolColor32.GetArray(vertexCount);
        Vector2[] uvs = DataPools.poolVector2.GetArray(vertexCount);

        for (int y = 0; y < height; y++)
        {
            float upRadius = circleHeights[y + 1];
            float downRadius = circleHeights[y];

            for (int x = fromX; x < toX; x++)
            {
                byte tileId = tilemapCircle.GetTile(x, y);

                if (tileId == 0) //skip empty tiles
                {
                    p1 = p2 = p3 = p4 = Vector3.zero;
                }
                else
                {
                    p1 = circleNormals[x] * upRadius;
                    p2 = circleNormals[(x + 1) % width] * upRadius;
                    p3 = circleNormals[(x + 1) % width] * downRadius;
                    p4 = circleNormals[x] * downRadius;
                }
                
                TileType tileType = tileTypes[tileId];

                vertices[vertexOffset + 0] = p1;
                vertices[vertexOffset + 1] = p2;
                vertices[vertexOffset + 2] = p3;
                vertices[vertexOffset + 3] = p4;
    
                //if (tilemapCircleView.debugColor)
                //{
                //    colors[vertexOffset + 0] = Color.red;
                //    colors[vertexOffset + 1] = Color.green;
                //    colors[vertexOffset + 2] = Color.blue;
                //    colors[vertexOffset + 3] = Color.cyan;
                //}
                //else
                //{
                    colors[vertexOffset + 0] = Color.white;
                    colors[vertexOffset + 1] = Color.white;
                    colors[vertexOffset + 2] = Color.white;
                    colors[vertexOffset + 3] = Color.white;
                //}
                
                TileSubtype subtype;
                
                if (y == height - 1 || tilemapCircle.GetTile(x, y + 1) == 0)
                    subtype = tileType.top;
                else
                    subtype = tileType.center;

                uvs[vertexOffset + 0].x = subtype.uvFromX;
                uvs[vertexOffset + 0].y = subtype.uvToY;
                
                uvs[vertexOffset + 1].x = subtype.uvToX;
                uvs[vertexOffset + 1].y = subtype.uvToY;
                
                uvs[vertexOffset + 2].x = subtype.uvToX;
                uvs[vertexOffset + 2].y = subtype.uvFromY;
                
                uvs[vertexOffset + 3].x = subtype.uvFromX;
                uvs[vertexOffset + 3].y = subtype.uvFromY;

                vertexOffset += 4;
                triangleOffset += 6;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.colors32 = colors;

        if (firstTime)
        {
            int[] triangles = DataPools.poolInt.GetArray(triangleCount);
        
            int size = height * (toX - fromX);
            
            vertexOffset = 0;
            triangleOffset = 0;
            
            for (int i = 0; i < size; i++)
            {
                triangles[triangleOffset + 0] = vertexOffset + 0;
                triangles[triangleOffset + 1] = vertexOffset + 1;
                triangles[triangleOffset + 2] = vertexOffset + 2;

                triangles[triangleOffset + 3] = vertexOffset + 2;
                triangles[triangleOffset + 4] = vertexOffset + 3;
                triangles[triangleOffset + 5] = vertexOffset + 0;
                
                triangleOffset += 6;
                vertexOffset += 4;
            }
            
            mesh.triangles = triangles;
            
            DataPools.poolInt.ReturnArray(triangles);
        }

        meshFilter.sharedMesh = mesh;
        
        DataPools.poolColor32.ReturnArray(colors);
        DataPools.poolVector3.ReturnArray(vertices);
        DataPools.poolVector2.ReturnArray(uvs);

        firstTime = false;
    }
}
