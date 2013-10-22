using System;

namespace UniverseEngine
{
    public struct TileSubtype
    {
        public float uvFromX;
        public float uvFromY;
        public float uvToX;
        public float uvToY;
    }
    
    public struct TileType
    {
        //public byte id; //Implitic in the position inside the array
        
        public TileSubtype center;
        
        public TileSubtype top;
    }
}

