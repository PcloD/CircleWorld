using UnityEngine;
using SpriteMeshEngine;

[ExecuteInEditMode]
public class SpriteView : MonoBehaviour
{
    public string spriteSheetId;
    public string spriteId;
    
    private Mesh mesh;
    
    public void Update()
    {
        if (!string.IsNullOrEmpty(spriteSheetId) && !string.IsNullOrEmpty(spriteId))
        {
            SpriteSheet spriteSheet = SpriteSheetManager.GetSpriteSheet(spriteSheetId);
            
            if (spriteSheet != null)
            {
                Sprite sprite = spriteSheet.GetSprite(spriteId);
                
                if (sprite != null)
                {
                    UpdateMesh(sprite);
                }
                else
                {
                    if (renderer)
                        renderer.enabled = false;
                }
            }
            else
            {
                if (renderer)
                    renderer.enabled = false;
            }
        }
        else
        {
            if (renderer)
                renderer.enabled = false;
        }
    }
    
    static private int[] triangles = new int[6];
    static private Vector2[] uvs = new Vector2[4];
    static private Vector3[] positions = new Vector3[4];
    
    private void UpdateMesh(Sprite sprite)
    {
        if (!mesh)
            mesh = new Mesh();
        
        if (!GetComponent<MeshRenderer>())
            gameObject.AddComponent<MeshRenderer>();
        
        if (!GetComponent<MeshFilter>())
            gameObject.AddComponent<MeshFilter>();
        
        renderer.sharedMaterial = sprite.SpriteSheet.Material;
        renderer.enabled = true;
        
        //float halfWidth = sprite.SizeX * 0.5f;
        //float halfHeight = sprite.SizeY * 0.5f;
        //positions[0] = new Vector3(-halfWidth, -halfHeight, 0.0f);
        //positions[1] = new Vector3(halfWidth, -halfHeight, 0.0f);
        //positions[2] = new Vector3(halfWidth, halfHeight, 0.0f);
        //positions[3] = new Vector3(-halfWidth, halfHeight, 0.0f);
        
        positions[0] = new Vector3(0, 0, 0.0f);
        positions[1] = new Vector3(sprite.SizeX, 0, 0.0f);
        positions[2] = new Vector3(sprite.SizeX, sprite.SizeY, 0.0f);
        positions[3] = new Vector3(0, sprite.SizeY, 0.0f);
        
        uvs[0] = new Vector2(sprite.UV.xMin, sprite.UV.yMin);
        uvs[1] = new Vector2(sprite.UV.xMax, sprite.UV.yMin);
        uvs[2] = new Vector2(sprite.UV.xMax, sprite.UV.yMax);
        uvs[3] = new Vector2(sprite.UV.xMin, sprite.UV.yMax);
        
        triangles[0] = 0;
        triangles[1] = 3;
        triangles[2] = 2;
                
        triangles[3] = 2;
        triangles[4] = 1;
        triangles[5] = 0;
        
        mesh.vertices = positions;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}


