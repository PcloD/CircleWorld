using UnityEngine;
using System.Collections.Generic;

namespace SpriteMeshEngine
{
    public class SpriteSheet
    {
        private string id;
        private SpriteDefinition[] sprites;
        private Texture2D texture;
        private Material defaultMaterial;
        
        public string Id
        {
            get { return id; }
        }
        
        public Texture2D Texture
        {
            get 
            { 
                if (texture == null)
                    texture = LoadTexture();
                
                return texture; 
            }
        }
                
        public SpriteSheet(string id, SpriteDefinition[] sprites)
        {
            this.id = id;
            this.sprites = sprites;
        }
        
        public int GetSpriteCount()
        {
            return sprites.Length;
        }
        
        public SpriteDefinition GetSpriteDefinition(int index)
        {
            return sprites[index];
        }
        
        public SpriteDefinition GetSpriteDefinition(string id)
        {
            for (int i = 0; i < sprites.Length; i++)
                if (sprites[i].Id == id)
                    return sprites[i];
            
            return null;
        }
        
        public Sprite GetSprite(int index)
        {
            return GetSpriteDefinition(index).Sprite;
        }

        public Sprite GetSprite(string id)
        {
            return GetSpriteDefinition(id).Sprite;
        }

        private Texture2D LoadTexture()
        {
            return (Texture2D) Resources.Load(SpriteSheetManager.SPRITE_SHEET_TEXTURE_FOLDER + id, typeof(Texture2D));
        }
        
        private Material CreateDefaultMaterial()
        {
            Material material = new Material(Shader.Find("SpriteMeshEngine/DefaultShader"));
            
            material.mainTexture = Texture;
            
            return material;
        }
        
        public void Clear()
        {
            if (texture)
            {
                Resources.UnloadAsset(texture);
                texture = null;
            }
            
            if (defaultMaterial)
            {
                if (Application.isEditor && !Application.isPlaying)
                    Object.DestroyImmediate(defaultMaterial);
                else
                    Object.Destroy(defaultMaterial);
                defaultMaterial = null;
            }
        }
    }
}
