using UnityEngine;
using SpriteMeshEngine;
using GUIEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class SpriteView : MonoBehaviour
{
    public string spriteSheetId;
    public string spriteId;
    
    public GUIAlignVertical verticalAlignment;
    public GUIAlignHorizontal horizontalAlignment;
    
    public int maxWidth = 0;
    public int maxHeight = 0;

    private SpriteMesh spriteMesh;
    
#if UNITY_EDITOR
    public void Update()
    {
        if (!Application.isPlaying)
            UpdateMesh();
    }
#endif
    
    public void Start()
    {
        UpdateMesh();
    }
    
    public void UpdateMesh(string spriteSheedId, string spriteId)
    {
        if (spriteSheedId != this.spriteSheetId || spriteId != this.spriteId)
        {
            this.spriteSheetId = spriteSheedId;
            this.spriteId = spriteId;
            
            UpdateMesh();
        }
    }
    
    public void UpdateMesh()
    {
        renderer.enabled = false;
        
        if (string.IsNullOrEmpty(spriteSheetId))
            return;
            
        if (string.IsNullOrEmpty(spriteId))
            return;
            
        SpriteSheet spriteSheet = SpriteSheetManager.GetSpriteSheet(spriteSheetId);
            
        if (spriteSheet == null)
            return;
        
        Sprite sprite = spriteSheet.GetSprite(spriteId);
        
        if (sprite == null)
            return;
        
        if (spriteMesh == null)
            spriteMesh = new SpriteMesh();

        renderer.sharedMaterial = sprite.SpriteSheet.DefaultMaterial;
        renderer.enabled = true;
        
        float sizeX = sprite.SizeX;
        float sizeY = sprite.SizeY;
        
        float x = 0;
        float y = 0;
        
        switch(verticalAlignment)
        {
            case GUIAlignVertical.Top:
                y = 0;
                break;
            case GUIAlignVertical.Center:
                y = -sizeY / 2.0f;
                break;
            case GUIAlignVertical.Bottom:
                y = -sizeY;
                break;
        }

        switch(horizontalAlignment)
        {
            case GUIAlignHorizontal.Left:
                x = 0;
                break;
            case GUIAlignHorizontal.Center:
                x = -sizeX / 2.0f;
                break;
            case GUIAlignHorizontal.Right:
                x = -sizeX;
                break;
        }
        
        spriteMesh.BeginCalculateSize();
        spriteMesh.AddSprite(sprite, x, y, sizeX, sizeY, Color.white);
        SpriteMeshInfo meshInfo = spriteMesh.End();
        spriteMesh.Begin(meshInfo);
        spriteMesh.AddSprite(sprite, x, y, sizeX, sizeY, Color.white);
        spriteMesh.End();
        spriteMesh.Apply();
        
        GetComponent<MeshFilter>().sharedMesh = spriteMesh.Mesh;
        
        if (maxWidth > 0 || maxHeight > 0)
        {
            float scaleX = 1.0f;
            float scaleY = 1.0f;
            
            if (maxWidth > 0)
                scaleX = sizeX / maxWidth;
            
            if (maxHeight > 0)
                scaleY = sizeY / maxHeight;
            
            float scale = Mathf.Max(scaleX, scaleY);
            if (scale < 0.01f)
                scale = 0.01f;
            
            transform.localScale = Vector3.one * (1.0f / scale);
        }
    }
    
    public void OnDrawGizmosSelected()
    {
        OnDrawGizmos();
    }
    
    public void OnDrawGizmos()
    {
        if (maxWidth > 0 || maxHeight > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(maxWidth / transform.localScale.x, maxHeight / transform.localScale.y, 1.0f));
        }
    }
}


