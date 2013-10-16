using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace SpriteMeshEngine
{
    public class SpriteSheetManager
    {
        public const string SPRITE_SHEET_TEXTURE_FOLDER = "Spritesheets/";
        public const string SPRITE_SHEET_DEFINITIONS_FILE = "Spritesheets/definitions";
        
        static private bool initialized;
        static private SpriteSheet[] spriteSheets;
        static private Dictionary<string, SpriteSheet> spriteSheetsById = new Dictionary<string, SpriteSheet>();
        
        static public SpriteSheet GetSpriteSheet(string id)
        {
            InitSpriteSheets();
            
            SpriteSheet spriteSheet;
            
            if (spriteSheetsById.TryGetValue(id, out spriteSheet))
                return spriteSheet;
            
            return null;
        }
        
        static public void Clear()
        {
            if (spriteSheets != null)
            {
                for (int i = 0; i < spriteSheets.Length; i++)
                    spriteSheets[i].Clear();
            
                spriteSheets = null;
            }
            
            spriteSheetsById.Clear();
            
            initialized = false;
        }
        
        static public void InitSpriteSheets()
        {
            if (initialized)
                return;
            
            initialized = true;
            
            TextAsset textAsset = (TextAsset) Resources.Load(SPRITE_SHEET_DEFINITIONS_FILE, typeof(TextAsset));
            
            if (textAsset)
            {
                spriteSheets = LoadSpriteSheetsFromBytes(textAsset.bytes);
                
                for (int i = 0; i < spriteSheets.Length; i++)
                    spriteSheetsById[spriteSheets[i].Id] = spriteSheets[i];
            }
        }
        
        static public SpriteSheet[] LoadSpriteSheetsFromBytes(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            BinaryReader reader = new BinaryReader(ms);
            
            int header = reader.ReadInt32();
            int version = reader.ReadInt32();
            
            if (header != 0xFAAF || version != 1)
                return new SpriteSheet[0];
            
            int spriteSheetsCount = reader.ReadInt32();
            
            SpriteSheet[] spriteSheets = new SpriteSheet[spriteSheetsCount];
            
            for (int i = 0; i < spriteSheetsCount; i++)
            {
                string id = reader.ReadString();
                int spriteCount = reader.ReadInt32();
                Sprite[] sprites = new Sprite[spriteCount];
                
                spriteSheets[i] = new SpriteSheet(id, sprites);
                
                for (int j = 0; j < spriteCount; j++)
                {
                    string spriteId = reader.ReadString();
                    int spriteSizeX = reader.ReadInt32();
                    int spriteSizeY = reader.ReadInt32();
                    float spriteUVx = reader.ReadSingle();
                    float spriteUVy = reader.ReadSingle();
                    float spriteUVwidth = reader.ReadSingle();
                    float spriteUVheight = reader.ReadSingle();
                    
                    sprites[j] = new Sprite(spriteId, spriteSizeX, spriteSizeY, new Rect(spriteUVx, spriteUVy, spriteUVwidth, spriteUVheight), spriteSheets[i]);
                }
            }
            
            
            return spriteSheets;
        }
        
        static public byte[] SaveSpriteSheeetsToBytes(SpriteSheet[] spriteSheets)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);
            
            writer.Write((int) 0xFAAF); //Header
            writer.Write((int) 1); //Version
            
            writer.Write((int) spriteSheets.Length);
            for (int i = 0; i < spriteSheets.Length; i++)
            {
                SpriteSheet spriteSheet = spriteSheets[i];
                
                writer.Write((string) spriteSheet.Id);
                writer.Write((int) spriteSheet.GetSpriteCount());
                
                for (int j = 0; j < spriteSheet.GetSpriteCount(); j++)
                {
                    Sprite sprite = spriteSheet.GetSprite(j);
                    
                    writer.Write((string) sprite.Id);
                    writer.Write((int) sprite.SizeX);
                    writer.Write((int) sprite.SizeY);
                    writer.Write((float) sprite.UV.x);
                    writer.Write((float) sprite.UV.y);
                    writer.Write((float) sprite.UV.width);
                    writer.Write((float) sprite.UV.height);
                }
            }
            
            writer.Flush();
            
            return ms.ToArray();
        }
    }
}

