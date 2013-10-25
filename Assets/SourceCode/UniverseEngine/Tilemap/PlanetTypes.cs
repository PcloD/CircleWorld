using System;
using SpriteMeshEngine;
using UnityEngine;

namespace UniverseEngine
{
    public class PlanetTypes
    {
        static private PlanetType[] planetTypes;
        static private SpriteSheet planetsSpriteSheet;
        
        static public PlanetType[] GetPlanetTypes()
        {
            if (planetTypes == null)
                InitPlanetTypes();
            
            return planetTypes;
        }
        
        static public PlanetType GetPlanetType(byte id)
        {
            return GetPlanetTypes()[id];
        }
        
        
        static private void InitPlanetTypes()
        {
            planetsSpriteSheet = SpriteSheetManager.GetSpriteSheet("Planets");
            planetTypes = new PlanetType[256];
            
            for (int i = 0; i < planetTypes.Length; i++)
            {
                planetTypes[i] = new PlanetType();
                planetTypes[i].id = (byte) i;
            }
            
            planetTypes[0].planetSprite = planetsSpriteSheet.GetSprite("planet-grass");
            planetTypes[0].mainTileId = 1;
            planetTypes[0].backColorFrom = new Color32(60, 179, 113, 255);
            planetTypes[0].backColorTo = new Color32(60, 179, 113, 0);
            
            planetTypes[1].planetSprite = planetsSpriteSheet.GetSprite("planet-sand");
            planetTypes[1].mainTileId = 2;
            planetTypes[1].backColorFrom = new Color32(238, 221, 130, 255);
            planetTypes[1].backColorTo = new Color32(238, 221, 130, 0);
            
            planetTypes[2].planetSprite = planetsSpriteSheet.GetSprite("planet-snow");
            planetTypes[2].mainTileId = 3;
            planetTypes[2].backColorFrom = new Color32(135, 206, 250, 255);
            planetTypes[2].backColorTo = new Color32(135, 206, 250, 0);
            
            planetTypes[3].planetSprite = planetsSpriteSheet.GetSprite("planet-stone");
            planetTypes[3].mainTileId = 4;
            planetTypes[3].backColorFrom = new Color32(153, 50, 204, 255);
            planetTypes[3].backColorTo = new Color32(153, 50, 204, 0);
        }
    }
}

