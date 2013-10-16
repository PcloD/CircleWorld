using System;

namespace UniverseEngine
{
    public interface IUniverseListener
    {
        void OnUniverseObjectAdded(UniverseObject universeObject);
        
        void OnPlanetReturned(Planet planet);
    }
}

