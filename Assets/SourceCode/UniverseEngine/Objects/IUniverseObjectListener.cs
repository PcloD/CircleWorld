using System;

namespace UniverseEngine
{
    public interface IUniverseObjectListener
    {
        void OnUniverseObjectUpdated(float deltaTime);
        
        void OnParentChanged(TilemapCircle parent);
    }
}

