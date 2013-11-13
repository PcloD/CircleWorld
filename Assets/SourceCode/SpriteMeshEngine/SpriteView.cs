using UnityEngine;
using SpriteMeshEngine;
using GUIEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteView : MonoBehaviour
{
    public string spriteSheetId;
    public string spriteId;
    
    public GUIAlignVertical verticalAlignment;
    public GUIAlignHorizontal horizontalAlignment;
    
    public int maxWidth = 0;
    public int maxHeight = 0;

    private SpriteRenderer spriteRenderer;

#if UNITY_EDITOR
    public void Update()
    {
        if (!Application.isPlaying)
            UpdateSprite();
    }
#endif
    
    public void Start()
    {
        UpdateSprite();
    }
    
    public void UpdateSprite(string spriteSheedId, string spriteId)
    {
        if (spriteSheedId != this.spriteSheetId || spriteId != this.spriteId)
        {
            this.spriteSheetId = spriteSheedId;
            this.spriteId = spriteId;
            
            UpdateSprite();
        }
    }
    
    public void UpdateSprite()
    {
        if (!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (string.IsNullOrEmpty(spriteSheetId))
        {
            spriteRenderer.enabled = false;
            return;
        }
            
        if (string.IsNullOrEmpty(spriteId))
        {
            spriteRenderer.enabled = false;
            return;
        }
            
        SpriteSheet spriteSheet = SpriteSheetManager.GetSpriteSheet(spriteSheetId);
            
        if (spriteSheet == null)
        {
            spriteRenderer.enabled = false;
            return;
        }
        
        SpriteMeshEngine.SpriteDefinition spriteDefinition = spriteSheet.GetSpriteDefinition(spriteId);
        
        if (spriteDefinition == null)
        {
            spriteRenderer.enabled = false;
            return;
        }

        spriteRenderer.sprite = spriteDefinition.Sprite;
        spriteRenderer.enabled = true;

        float sizeX = spriteDefinition.SizeX;
        float sizeY = spriteDefinition.SizeY;

        Vector3 localScale = transform.localScale;

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

            localScale = Vector3.one * (1.0f / scale);
            transform.localScale = localScale;
        }

        if (verticalAlignment != GUIAlignVertical.None || horizontalAlignment != GUIAlignHorizontal.None)
        {
            float x = 0;
            float y = 0;
            
            switch(verticalAlignment)
            {
                case GUIAlignVertical.Bottom:
                    y = 0;
                    break;
                case GUIAlignVertical.Center:
                    y = (-sizeY / 2.0f) * localScale.y;
                    break;
                case GUIAlignVertical.Top:
                    y = -sizeY * localScale.y;
                    break;
            }

            switch(horizontalAlignment)
            {
                case GUIAlignHorizontal.Left:
                    x = 0;
                    break;
                case GUIAlignHorizontal.Center:
                    x = (-sizeX / 2.0f) * localScale.x;
                    break;
                case GUIAlignHorizontal.Right:
                    x = -sizeX * localScale.x;
                    break;
            }

            float localRotation = transform.localRotation.eulerAngles.z * Mathf.Deg2Rad;
            
            float cosLocalRotation = Mathf.Cos(localRotation);
            float sinLocalRotation = Mathf.Sin(localRotation);

            transform.localPosition = new Vector3(
                x * cosLocalRotation - y * sinLocalRotation, 
                x * sinLocalRotation + y * cosLocalRotation, 
                0);
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
            Gizmos.DrawWireCube(new Vector3(-transform.localPosition.x / transform.localScale.x, -transform.localPosition.y / transform.localScale.y, 0.0f), new Vector3(maxWidth / transform.localScale.x, maxHeight / transform.localScale.y, 1.0f));
        }
    }
}


