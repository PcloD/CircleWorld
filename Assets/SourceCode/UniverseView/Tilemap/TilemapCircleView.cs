using UnityEngine;
using UniverseEngine;
using System.Collections.Generic;

public class TilemapCircleView : MonoBehaviour, ITilemapCircleListener
{
    public bool debugColor;
    public Material material;
    public Material backgroundMaterial;
    
    private int lastHeight;
    private int lastWidth;
    
    private TilemapCircleViewBackgroundRenderer backgroundRenderer;

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
        
        InitRenderers();
        
        UpdateMesh();
    }
 
    private void UpdateMesh()
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].UpdateMesh();
    }

    private void InitRenderers()
    {
        int renderersAmount = Mathf.Clamp(Mathf.CeilToInt((tilemapCircle.Width * tilemapCircle.Height) / (32 * 32)), 1, 256);

        if (renderers == null || renderers.Length != renderersAmount || lastWidth != tilemapCircle.Width || lastHeight != tilemapCircle.Height)
        {
            //Destroy existing renderers
            if (renderers != null)
                foreach (TilemapCircleViewRenderer rend in renderers)
                    if (rend)
                        GameObject.Destroy(rend.gameObject);
            
            if (backgroundRenderer)
                GameObject.Destroy(backgroundRenderer.gameObject);
   
            //Add tile map circle renderers
            renderers = new TilemapCircleViewRenderer[renderersAmount];

            lastWidth = tilemapCircle.Width;
            lastHeight = tilemapCircle.Height;

            int sizeX = Mathf.CeilToInt((float) tilemapCircle.Width / (float) renderers.Length);

            int fromX = 0;
            int toX = sizeX;

            for (int i = 0; i < renderers.Length; i++)
            {
                GameObject go = new GameObject("Renderer " + i);
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                
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
            
            //Add background renderer
            GameObject goBack = new GameObject("BackRenderer");
            goBack.transform.parent = transform;
            goBack.transform.localPosition = Vector3.zero;
            goBack.transform.localRotation = Quaternion.identity;
            goBack.transform.localScale = Vector3.one;
            
            backgroundRenderer = goBack.AddComponent<TilemapCircleViewBackgroundRenderer>();
        }
        
        backgroundRenderer.Init(this);

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
