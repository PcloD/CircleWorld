using System;

namespace UniverseEngine
{
    public interface ITilemapCircleListener
    {
        void OnTilemapTileChanged(int tileX, int tileY);
        
        void OnTilemapParentChanged(float deltaTime);
    }
}
