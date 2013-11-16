using UnityEngine;
using System.Collections;
using SpriteMeshEngine;

public class Benchmark : MonoBehaviour 
{
    private Mesh mesh1;
    private SpriteSheet spriteSheet;
    private MeshFilter meshFilter;
    private Camera mainCamera;

	// Use this for initialization
	void Start () 
    {
        mainCamera = Camera.main;

        spriteSheet = SpriteMeshEngine.SpriteSheetManager.GetSpriteSheet("Planets");

        renderer.sharedMaterial.mainTexture = spriteSheet.Texture;

        meshFilter = GetComponent<MeshFilter>();
        
        mesh1 = new Mesh();

        meshFilter.sharedMesh = mesh1;

        UpdateMesh(true);
    }
	
	// Update is called once per frame
	void Update () 
    {
        UpdateMesh(false);
    }

    private Vector3[] vertices;
    private Vector2[] uvs;

    private void UpdateMesh(bool firstTime)
    {
        if (mainCamera.orthographicSize !=  Screen.height / 2)
            mainCamera.orthographicSize = Screen.height / 2;

        int rects = 4096;

        int vertexCount = rects * 4;
        int triangleCount = rects * 6;

        if (vertices == null)
            vertices = new Vector3[vertexCount];

        if (uvs == null)
            uvs = new Vector2[vertexCount];

        int vertexOffset = 0;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float radius = 10 / 2;

        //Update all positions
        for (int i = 0; i < rects; i++)
        {
            float x = Random.Range(-screenWidth / 2, screenWidth / 2);
            float y = Random.Range(-screenHeight / 2, screenHeight / 2);

            vertices[vertexOffset + 0].x = x - radius;
            vertices[vertexOffset + 0].y = y - radius;
            
            vertices[vertexOffset + 1].x = x - radius;
            vertices[vertexOffset + 1].y = y + radius;
            
            vertices[vertexOffset + 2].x = x + radius;
            vertices[vertexOffset + 2].y = y + radius;
            
            vertices[vertexOffset + 3].x = x + radius;
            vertices[vertexOffset + 3].y = y - radius;
            
            vertexOffset += 4;
        }

        vertexOffset = 0;

        for (ushort i = 0; i < rects; i++)
        {
            SpriteDefinition sprite = spriteSheet.GetSpriteDefinition(Random.Range(0, spriteSheet.GetSpriteCount()));
            
            Rect planetUV = sprite.UV;
            
            uvs[vertexOffset + 0] = new Vector2(planetUV.xMin, planetUV.yMax);
            uvs[vertexOffset + 1] = new Vector2(planetUV.xMax, planetUV.yMax);
            uvs[vertexOffset + 2] = new Vector2(planetUV.xMax, planetUV.yMin);
            uvs[vertexOffset + 3] = new Vector2(planetUV.xMin, planetUV.yMin);

            vertexOffset += 4;
        }

        if (firstTime)
        {
            //Update triangles and uvs only the first time that the mesh is updated
            
            int triangleOffset = 0;
            vertexOffset = 0;
            
            int[] triangles = new int[triangleCount];

            for (ushort i = 0; i < rects; i++)
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
            
            mesh1.vertices = vertices;
            mesh1.uv = uvs;
            mesh1.triangles = triangles;
            mesh1.bounds = new Bounds(Vector3.zero, new Vector3(ushort.MaxValue * 2, ushort.MaxValue * 2, 0.0f));
            
            mesh1.Optimize();
        }
        else
        {
            mesh1.vertices = vertices;
            mesh1.uv = uvs;
        }
	}
}
