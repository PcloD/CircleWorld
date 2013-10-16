using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using SpriteMeshEngine;

public class SpriteSheetGenerator
{
    [MenuItem("Sprite Mesh Engine/Update Spritesheets")]
    static public void UpdateSpritesheets()
    {
		Debug.Log("--- Starting spritesheet update ---");
		
		string[] directories = Directory.GetDirectories("Assets/Spritesheets");
		
		List<SpriteSheet> spriteSheets = new List<SpriteSheet>();
		
		foreach(string directory in directories)
		{
			Debug.Log("Processing directory " + directory);
			
			string[] files = Directory.GetFiles(directory);
			
			List<Texture2D> spriteTextures = new List<Texture2D>();
			List<string> spriteIds = new List<string>();
			
			foreach(string file in files)
			{
				string extension = Path.GetExtension(file).ToUpperInvariant();
				
				if (extension == ".JPG" || extension == ".PNG" || extension == ".TGA")
				{
					Texture2D texture = AssetDatabase.LoadAssetAtPath(file, typeof(Texture2D)) as Texture2D;
					
					if (texture)
					{
						spriteTextures.Add(texture);
						spriteIds.Add(Path.GetFileNameWithoutExtension(file));
					}
				}
			}
			
			if (spriteTextures.Count > 0)
			{
				Texture2D packedTexture = new Texture2D(1024, 1024);
				
				Rect[] uvs = packedTexture.PackTextures(spriteTextures.ToArray(), 2);
				
				byte[] packedTextureBytes = packedTexture.EncodeToPNG();
				
				string spriteSheetId = Path.GetFileName(directory);
				
				string packedTextureFilename = "Assets/Resources/Spritesheets/" + spriteSheetId + ".png";
				
				File.WriteAllBytes(packedTextureFilename, packedTextureBytes);
				
				Sprite[] sprites = new Sprite[spriteTextures.Count];
				
				SpriteSheet spriteSheet = new SpriteSheet(spriteSheetId, sprites);
				
				for (int i = 0; i < sprites.Length; i++)
					sprites[i] = new Sprite(spriteIds[i], spriteTextures[i].width, spriteTextures[i].height, uvs[i], spriteSheet);
				spriteSheets.Add(spriteSheet);
				
				Debug.Log(packedTextureFilename + " : Added " + sprites.Length + " Sprites");
			}
		}
		
		File.WriteAllBytes("Assets/Resources/Spritesheets/definitions.bytes", SpriteSheetManager.SaveSpriteSheeetsToBytes(spriteSheets.ToArray()));
		
		SpriteSheetManager.Clear();
		
		Debug.Log("--- Finished spritesheet update ---");
		
		AssetDatabase.Refresh();
    }
	
	
}


