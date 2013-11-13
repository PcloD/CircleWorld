using UnityEngine;
using SpriteMeshEngine;

[ExecuteInEditMode]
public class SpriteMeshTest : SpriteMeshView
{
    protected override SpriteSheet GetSpritesheet()
    {
        return SpriteSheetManager.GetSpriteSheet("Tilemap");
    }

    protected override void DrawMesh()
    {
        float offsetX = 0;
        float offsetY = 0;

        spriteMesh.SetScale(scale);

        for (int j = 0; j < spriteSheet.GetSpriteCount(); j++)
        {
            offsetX = 0;

            for (int i = 0; i < spriteSheet.GetSpriteCount(); i++)
            {
                SpriteMeshEngine.SpriteDefinition sprite = spriteSheet.GetSpriteDefinition((i + j) % spriteSheet.GetSpriteCount());

                spriteMesh.AddSprite(sprite, offsetX, offsetY, sprite.SizeX, sprite.SizeY, Color.white);

                offsetX += sprite.SizeX;
            }

            offsetY += 70;
        }
    }
}


