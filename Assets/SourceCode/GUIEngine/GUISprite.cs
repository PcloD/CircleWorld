using UnityEngine;
using SpriteMeshEngine;

namespace GUIEngine
{
    public class GUISprite : GUIObject
    {
        public string spriteId = "";

        private Sprite sprite;

        protected override void OnInit()
        {
            if (!string.IsNullOrEmpty(spriteId))
                sprite = panel.SpriteSheet.GetSprite(spriteId);
            else
                sprite = null;
        }

        protected override void OnDraw(SpriteMesh spriteMesh, float x, float y)
        {
            if (sprite != null)
                spriteMesh.AddSprite(sprite, x, y, sprite.SizeX, sprite.SizeY, Color.white);
        }

        protected override Vector2 OnGetSize()
        {
            if (sprite != null)
                return new Vector2(sprite.SizeX, sprite.SizeY);
            else
                return new Vector2(10, 10);
        }
    }
}

