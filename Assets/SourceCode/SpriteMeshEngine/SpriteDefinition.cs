using UnityEngine;
using System.Collections.Generic;

namespace SpriteMeshEngine
{
    public class SpriteDefinition
    {
        private string id;
        private Rect uv;
        private int sizeX;
        private int sizeY;
        private SpriteSheet spriteSheet;
        private Sprite sprite;
        
        public string Id
        {
            get { return id; }
        }
        
        public Rect UV
        {
            get { return uv; }
        }
        
        public int SizeX
        {
            get { return sizeX; }
        }
        
        public int SizeY
        {
            get { return sizeY; }
        }
        
        public SpriteSheet SpriteSheet
        {
            get { return spriteSheet; }
        }

        public Sprite Sprite
        {
            get
            {
                if (sprite == null)
                {
                    sprite = SpriteSheetManager.GetSpriteCache().GetSprite(SpriteSheet.Id, Id);

                    if (sprite == null)
                    {
                        Debug.LogWarning("Sprite not found in sprite cache!!");

                        Rect rect = uv;
                        rect.x *= SpriteSheet.Texture.width;
                        rect.y *= SpriteSheet.Texture.height;
                        rect.width *= SpriteSheet.Texture.width;
                        rect.height *= SpriteSheet.Texture.height;

                        sprite = Sprite.Create(SpriteSheet.Texture, rect, Vector2.zero, 1, 0, SpriteMeshType.FullRect);
                    }
                }

                return sprite;
            }

        }
        
        public SpriteDefinition(string id, int sizeX, int sizeY, Rect uv, SpriteSheet spriteSheet)
        {
            this.id = id;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.uv = uv;
            this.spriteSheet = spriteSheet;
        }

    }
}
