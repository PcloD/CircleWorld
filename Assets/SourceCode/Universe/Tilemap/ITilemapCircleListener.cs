using System;

namespace Universe
{
    public interface ITilemapCircleListener
    {
        void OnTileChange(int tileX, int tileY);
    }
}
