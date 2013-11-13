using System;
using SpriteMeshEngine;

namespace UniverseEngine
{
    public class TileTypes
    {
        static private TileType[] tileTypes;
        static private SpriteSheet tilemapSpriteSheet;
        
        static public TileType[] GetTileTypes()
        {
            if (tileTypes == null)
                InitTileTypes();
            
            return tileTypes;
        }
        
        static public TileType GetTileType(byte id)
        {
            return GetTileTypes()[id];
        }
        
        static private void InitTileTypes()
        {
            tilemapSpriteSheet = SpriteSheetManager.GetSpriteSheet("Tilemap");
            
            tileTypes = new TileType[256];
            
            tileTypes[1].center = GetTileSubtypeUV("grassCenter");
            tileTypes[1].top = GetTileSubtypeUV("grassMid");
            
            tileTypes[2].center = GetTileSubtypeUV("sandCenter");
            tileTypes[2].top = GetTileSubtypeUV("sandMid");

            tileTypes[3].center = GetTileSubtypeUV("snowCenter");
            tileTypes[3].top = GetTileSubtypeUV("snowMid");

            tileTypes[4].center = GetTileSubtypeUV("stoneCenter");
            tileTypes[4].top = GetTileSubtypeUV("stoneMid");
            
            tileTypes[5].center = GetTileSubtypeUV("sun1Center");
            tileTypes[5].top = GetTileSubtypeUV("sun1Mid");
            
            tileTypes[6].center = GetTileSubtypeUV("sun2Center");
            tileTypes[6].top = GetTileSubtypeUV("sun2Mid");
        }
        
        static private TileSubtype GetTileSubtypeUV(string id)
        {
            TileSubtype subtype = new TileSubtype();
            
            UnityEngine.Rect rect = tilemapSpriteSheet.GetSpriteDefinition(id).UV;
            
            subtype.uvFromX = rect.xMin;
            subtype.uvToX = rect.xMax;
            subtype.uvFromY = rect.yMin;
            subtype.uvToY = rect.yMax;
            
            return subtype;
        }
    }
}

