using UnityEngine;
using System.Collections.Generic;

namespace SpriteMeshEngine
{
    public class Sprite
    {
        private string id;
        private Rect uv;
        private int sizeX;
        private int sizeY;
        private SpriteSheet spriteSheet;
        
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
        
        public Sprite(string id, int sizeX, int sizeY, Rect uv, SpriteSheet spriteSheet)
        {
            this.id = id;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.uv = uv;
            this.spriteSheet = spriteSheet;
        }
    }
}
