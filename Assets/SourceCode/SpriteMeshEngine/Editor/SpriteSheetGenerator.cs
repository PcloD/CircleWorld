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
				
				SpriteMeshEngine.SpriteDefinition[] sprites = new SpriteMeshEngine.SpriteDefinition[spriteTextures.Count];
				
				SpriteSheet spriteSheet = new SpriteSheet(spriteSheetId, sprites);
				
				for (int i = 0; i < sprites.Length; i++)
					sprites[i] = new SpriteMeshEngine.SpriteDefinition(spriteIds[i], spriteTextures[i].width, spriteTextures[i].height, uvs[i], spriteSheet);
				spriteSheets.Add(spriteSheet);
				
				Debug.Log(packedTextureFilename + " : Added " + sprites.Length + " Sprites");
			}
		}
		
		File.WriteAllBytes("Assets/Resources/Spritesheets/definitions.bytes", SpriteSheetManager.SaveSpriteSheeetsToBytes(spriteSheets.ToArray()));
		
		SpriteSheetManager.Clear();
		
		Debug.Log("--- Finished spritesheet update ---");
		
		AssetDatabase.Refresh();

		Debug.Log("--- Generating Unity Sprites ---");

		for (int i = 0; i < SpriteSheetManager.GetSpriteSheetCount(); i++)
		{
			SpriteSheet spriteSheet = SpriteSheetManager.GetSpriteSheet(i);

			Texture2D spriteSheetTexture = spriteSheet.Texture;

			string path = AssetDatabase.GetAssetPath(spriteSheetTexture);
			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
						
			//Disable spritesheet and reimport, if we don't do this, Unity won't take any changes made to the spritesheet definition (BUG in unity?)
			textureImporter.spriteImportMode = SpriteImportMode.None;
			AssetDatabase.ImportAsset(path);
			
			//Now calculate the new spritesheets
			SpriteMetaData[] spriteMetaDatas = new SpriteMetaData[spriteSheet.GetSpriteCount()];
			
			int textureWidth = spriteSheetTexture.width;
			int textureHeight = spriteSheetTexture.height;
			
			if ((textureWidth >= textureHeight) && textureWidth > textureImporter.maxTextureSize)
			{
				float scale = textureWidth / textureImporter.maxTextureSize;
				
				textureWidth = textureImporter.maxTextureSize;
				textureHeight = Mathf.RoundToInt(textureHeight / scale);
			}
			else if ((textureHeight >= textureWidth) && textureHeight > textureImporter.maxTextureSize)
			{
				float scale = textureHeight / textureImporter.maxTextureSize;
				
				textureHeight = textureImporter.maxTextureSize;
				textureWidth = Mathf.RoundToInt(textureWidth / scale);
			}
			
			for (int j = 0; j < spriteSheet.GetSpriteCount(); j++)
			{
				SpriteDefinition spriteDefinition = spriteSheet.GetSpriteDefinition(j);
				
				SpriteMetaData metaData = new SpriteMetaData();

				metaData.name = spriteDefinition.Id;
				metaData.alignment = (int) SpriteAlignment.BottomLeft;
				metaData.pivot = Vector2.zero;
				metaData.rect = spriteDefinition.UV;
				metaData.rect.x *= textureWidth;
				metaData.rect.y *= textureHeight;
				metaData.rect.width *= textureWidth;
				metaData.rect.height *= textureHeight;
				
				spriteMetaDatas[j] = metaData;
			}

			textureImporter.textureType = TextureImporterType.Sprite;
			textureImporter.npotScale = TextureImporterNPOTScale.None;
			textureImporter.alphaIsTransparency = true;
			textureImporter.spriteImportMode = SpriteImportMode.Multiple;
			textureImporter.spritePixelsToUnits = 1;
			textureImporter.spritePackingTag = "";
			textureImporter.spritePivot = Vector2.zero;
			textureImporter.spritesheet = spriteMetaDatas;
			
			AssetDatabase.ImportAsset(path);
		}

		Debug.Log("--- Finished generating Unity Sprites ---");

		Debug.Log("--- Updating sprite cache ---");

		GameObject spriteCachePrefab = Resources.Load<GameObject>("Spritesheets/SpriteCache");

		List<SpriteCacheEntry> cacheEntries = new List<SpriteCacheEntry>();

		if (spriteCachePrefab)
		{
			SpriteCache spriteCache = spriteCachePrefab.GetComponent<SpriteCache>();

			for (int i = 0; i < SpriteSheetManager.GetSpriteSheetCount(); i++)
			{
				SpriteSheet spriteSheet = SpriteSheetManager.GetSpriteSheet(i);
				
				Texture2D spriteSheetTexture = spriteSheet.Texture;
				
				string path = AssetDatabase.GetAssetPath(spriteSheetTexture);

				List<Sprite> sprites = new List<Sprite>();

				foreach(Object obj in AssetDatabase.LoadAllAssetsAtPath(path))
					if (obj is Sprite)
						sprites.Add((Sprite) obj);

				SpriteCacheEntry cacheEntry = new SpriteCacheEntry();
				cacheEntry.spritesheetId = spriteSheet.Id;
				cacheEntry.sprites = sprites.ToArray();

				cacheEntries.Add(cacheEntry);
			}

			spriteCache.cacheEntries = cacheEntries.ToArray();

			EditorUtility.SetDirty(spriteCache);
		}
		else
		{
			Debug.LogError("Sprite cache prefab not found!!!");
		}

		Debug.Log("--- Finished updating sprite cache ---");
    }	
}


