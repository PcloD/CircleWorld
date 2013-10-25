using UnityEngine;
using SpriteMeshEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SpriteMeshView : MonoBehaviour
{
    public float scale = 1;

    private int lastScreenWidth = -1;
    private int lastScreenHeight = -1;
    private float lastScale = -1;
    private bool dirty = true;

    protected SpriteMesh spriteMesh;
    protected SpriteSheet spriteSheet;

    protected MeshFilter meshFilter;

    public SpriteSheet SpriteSheet
    {
        get 
        { 
            if (spriteSheet == null)
            {
                spriteSheet = GetSpritesheet();
                
                renderer.sharedMaterial = spriteSheet.DefaultMaterial;
            }

            return spriteSheet; 
        }
    }

    public virtual void Start()
    {
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        Refresh();
    }

    public virtual void Update()
    {
        if (dirty || scale != lastScale || Screen.height != lastScreenHeight || Screen.width != lastScreenWidth)
            Refresh();
    }

    public void SetDirty()
    {
        dirty = true;
    }

    public void Refresh()
    {
        if (Application.isPlaying)
            transform.position = new Vector3(-Screen.width / 2, Screen.height / 2, 0);

        if (SpriteSheet == null)
            return;

        if (spriteMesh == null)
            spriteMesh = new SpriteMesh();

        if (scale != lastScale || dirty)
        {
            dirty = false;

            spriteMesh.BeginCalculateSize();
            DrawMesh();
            SpriteMeshInfo meshInfo = spriteMesh.End();

            spriteMesh.Begin(meshInfo);
            DrawMesh();
            spriteMesh.End();

            spriteMesh.Apply();
            GetComponent<MeshFilter>().sharedMesh = spriteMesh.Mesh;

            lastScale = scale;
        }

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }

    protected virtual SpriteSheet GetSpritesheet()
    {
        return null;
    }

    protected virtual void DrawMesh()
    {
    }
}


