using UnityEngine;
using UniverseEngine;
using System.Collections.Generic;

public class TilemapCircleViewBackgroundRenderer : MonoBehaviour
{
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    
    private TilemapCircleView tilemapCircleView;
    private TilemapCircle tilemapCircle;
    
    private Vector2[] circleNormals;
    private float[] circleHeights;
    
    private bool firstTime = true;

    public void Awake()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        mesh = new Mesh();
    }
    
    public void Init(TilemapCircleView tilemapCircleView)
    {
        this.tilemapCircleView = tilemapCircleView;
        this.tilemapCircle = tilemapCircleView.TilemapCircle;
        this.circleNormals = tilemapCircle.CircleNormals;
        this.circleHeights = tilemapCircle.CircleHeights;
        
        meshRenderer.sharedMaterial = this.tilemapCircleView.backgroundMaterial;
        
        InitMesh();
    }

    private void InitMesh()
    {
        Color32 fromColor = Color.blue;
        Color32 toColor = Color.cyan;
        toColor.a = 0;
        
        if (tilemapCircle is Planet)
        {
            PlanetType planetType = ((Planet) tilemapCircle).PlanetType;
            
            fromColor = planetType.backColorFrom;
            toColor = planetType.backColorTo;
        }
        
        
        int vertexCount = circleNormals.Length + 1;
        int triangleCount = circleNormals.Length * 3;
        
        Vector3[] vertices = DataPools.poolVector3.GetArray(vertexCount);
        Color32[] colors = DataPools.poolColor32.GetArray(vertexCount);
        //Vector2[] uvs = DataPools.poolVector2.GetArray(vertexCount);
        
        float heavenHeight = circleHeights[circleHeights.Length - 1] + 
                             (circleHeights[circleHeights.Length - 1] - circleHeights[circleHeights.Length - 2]) * 5.0f; //Repeat the last tile height 5 times
        
        
        int vertexOffset = 1;
        vertices[0] = Vector3.zero;
        vertices[0].z = 10;
        colors[0] = fromColor;
          
        for (int i = 0; i < circleNormals.Length; i++)
        {
            vertices[vertexOffset] = circleNormals[i] * heavenHeight;
            vertices[vertexOffset].z = 10;
            colors[vertexOffset] = toColor;
            
            //uvs[vertexOffset] = Vector2.zero;
                        
            vertexOffset++;
        }
  
        mesh.vertices = vertices;
        //mesh.uv = uvs;
        mesh.colors32 = colors;
        
        if (firstTime)
        {
            firstTime = false;
            
            int[] triangles = DataPools.poolInt.GetArray(triangleCount);
            
            int triangleOffset = 0;
            vertexOffset = 1;
            
            for (int i = 1; i < circleNormals.Length; i++)
            {
                triangles[triangleOffset + 0] = 0;
                triangles[triangleOffset + 1] = vertexOffset - 1;
                triangles[triangleOffset + 2] = vertexOffset;
                
                triangleOffset += 3;
                vertexOffset ++;
            }
            
            triangles[triangleOffset + 0] = 0;
            triangles[triangleOffset + 1] = vertexOffset - 1;
            triangles[triangleOffset + 2] = 1;            
            
            mesh.triangles = triangles;
            mesh.Optimize();
            
            DataPools.poolInt.ReturnArray(triangles);
        }
        

        meshFilter.sharedMesh = mesh;
        
        DataPools.poolColor32.ReturnArray(colors);
        DataPools.poolVector3.ReturnArray(vertices);
        //DataPools.poolVector2.ReturnArray(uvs);
    }
}
