using UnityEngine;
using System.Collections.Generic;

namespace SpriteMeshEngine
{
    public class SpriteSheet
    {
        private string id;
        private Sprite[] sprites;
        private Texture2D texture;
        private Material material;
        
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
        
        public Material Material
        {
            get
            {
                if (material == null)
                    material = CreateMaterial();
                
                return material;
            }
        }
        
        public SpriteSheet(string id, Sprite[] sprites)
        {
            this.id = id;
            this.sprites = sprites;
        }
        
        public int GetSpriteCount()
        {
            return sprites.Length;
        }
        
        public Sprite GetSprite(int index)
        {
            return sprites[index];
        }
        
        public Sprite GetSprite(string id)
        {
            for (int i = 0; i < sprites.Length; i++)
                if (sprites[i].Id == id)
                    return sprites[i];
            
            return null;
        }
        
        private Texture2D LoadTexture()
        {
            return (Texture2D) Resources.Load(SpriteSheetManager.SPRITE_SHEET_TEXTURE_FOLDER + id, typeof(Texture2D));
        }
        
        private Material CreateMaterial()
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
            
            if (material)
            {
                Object.Destroy(material);
                material = null;
            }
        }
    }
}
