using System;
using SpriteMeshEngine;
using UnityEngine;

namespace UniverseEngine
{
    public class PlanetType
    {
        public byte id;
        
        public byte mainTileId;
        
        public SpriteMeshEngine.SpriteDefinition planetSprite;
        
        public Color32 backColorFrom;
        public Color32 backColorTo;
    }
}

