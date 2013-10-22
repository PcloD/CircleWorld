using UnityEngine;
using UniverseEngine;
using System.Collections.Generic;

public class TilemapCircleView : MonoBehaviour, ITilemapCircleListener
{
    public bool debugColor;
    public Material material;
    
    private int lastHeight;
    private int lastWidth;

    private TilemapCircleViewRenderer[] renderers;

    private TilemapCircle tilemapCircle;
    
    private Transform trans;
    
    public TilemapCircle TilemapCircle
    {
        get { return tilemapCircle; }
    }
    
    public void Awake()
    {
        if (!material.mainTexture)
            material.mainTexture = SpriteMeshEngine.SpriteSheetManager.GetSpriteSheet("Tilemap").Texture;
        
        trans = transform;
    }
    
    public void Init(TilemapCircle tilemapCircle)
    {
        this.tilemapCircle = tilemapCircle;
        
        tilemapCircle.Listener = this;
        
        UpdatePosition();
        
        UpdateRenderers();
        
        UpdateMesh();
    }
 
    private void UpdateMesh()
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].UpdateMesh();
    }

    private void UpdateRenderers()
    {
        int renderersAmount = Mathf.Clamp(Mathf.CeilToInt((tilemapCircle.Width * tilemapCircle.Height) / (32 * 32)), 1, 256);

        bool recreate = false;

        if (Application.isEditor && !Application.isPlaying)
        {
            if (renderers != null)
            {
                foreach (TilemapCircleViewRenderer rend in renderers)
                    if (!rend)
                        recreate = true;
            }
        }

        if (renderers == null || renderers.Length != renderersAmount || recreate || lastWidth != tilemapCircle.Width || lastHeight != tilemapCircle.Height)
        {
            if (renderers != null)
            {
                foreach (TilemapCircleViewRenderer rend in renderers)
                {
                    if (rend)
                    {
                        if (Application.isPlaying)
                            GameObject.Destroy(rend.gameObject);
                        else
                            GameObject.DestroyImmediate(rend.gameObject);
                    }
                }
            }

            renderers = new TilemapCircleViewRenderer[renderersAmount];

            lastWidth = tilemapCircle.Width;
            lastHeight = tilemapCircle.Height;

            int sizeX = Mathf.CeilToInt((float) tilemapCircle.Width / (float) renderers.Length);

            int fromX = 0;
            int toX = sizeX;

            for (int i = 0; i < renderers.Length; i++)
            {
                GameObject go = new GameObject("Renderer " + i);
                go.hideFlags = HideFlags.DontSave;
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                go.AddComponent<MeshRenderer>().sharedMaterial = material;

                renderers[i] = go.AddComponent<TilemapCircleViewRenderer>();

                renderers[i].Init(
                    this, 
                    fromX,
                    toX);

                fromX += sizeX;
                toX += sizeX;

                if (toX >= tilemapCircle.Width)
                    toX = tilemapCircle.Width;
                
                if (fromX > toX)
                    fromX = toX;
            }
        }

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].SetDirty();
    }

    private TilemapCircleViewRenderer GetRenderer(int tileX, int tileY)
    {
        if (renderers != null && renderers.Length > 0)
        {
            int sizeX = Mathf.CeilToInt((float) tilemapCircle.Width / (float) renderers.Length);

            int rendererIndex = tileX / sizeX;

            return renderers[rendererIndex];
        }

        return null;
    }

    public void OnTilemapTileChanged (int tileX, int tileY)
    {
        GetRenderer(tileX, tileY).SetDirty();
    }
    
    public void OnTilemapParentChanged(float deltaTime)
    {
        UpdatePosition();
        
        UpdateMesh();
    }
    
    private void UpdatePosition()
    {
        trans.localPosition = tilemapCircle.Position;
        trans.localRotation = Quaternion.AngleAxis(-tilemapCircle.Rotation * Mathf.Rad2Deg, Vector3.forward);
    }
    
    public virtual void Recycle()
    {
        tilemapCircle = null;
        
        gameObject.SetActive(false);
    }
}
