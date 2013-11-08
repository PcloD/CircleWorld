using UnityEngine;
using SpriteMeshEngine;

[ExecuteInEditMode]
public class SpriteView : MonoBehaviour
{
    public string spriteSheetId;
    public string spriteId;

    private SpriteMesh spriteMesh;
    
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
    
    private void UpdateMesh(Sprite sprite)
    {
        if (spriteMesh == null)
            spriteMesh = new SpriteMesh();

        if (!GetComponent<MeshRenderer>())
            gameObject.AddComponent<MeshRenderer>();
        
        if (!GetComponent<MeshFilter>())
            gameObject.AddComponent<MeshFilter>();
        
        renderer.sharedMaterial = sprite.SpriteSheet.DefaultMaterial;
        renderer.enabled = true;

        spriteMesh.BeginCalculateSize();
        spriteMesh.AddSprite(sprite, 0, -sprite.SizeY, sprite.SizeX, sprite.SizeY, Color.white);
        SpriteMeshInfo meshInfo = spriteMesh.End();
        spriteMesh.Begin(meshInfo);
        spriteMesh.AddSprite(sprite, 0, -sprite.SizeY, sprite.SizeX, sprite.SizeY, Color.white);
        spriteMesh.End();
        spriteMesh.Apply();

        GetComponent<MeshFilter>().sharedMesh = spriteMesh.Mesh;
    }
}


