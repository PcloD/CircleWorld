using System;
using System.Collections.Generic;

namespace UniverseEngine
{
    public class UniverseFactory
    {
        private Dictionary<int, List<Planet>> planetsPool = new Dictionary<int, List<Planet>>();
        
        public Avatar GetAvatar()
        {
            return new Avatar();
        }
        
        public Ship GetShip()
        {
            return new Ship();
        }
        
        public Planet GetPlanet(int height)
        {
            Planet planet = null;
            
            List<Planet> list;
            
            if (planetsPool.TryGetValue(height, out list) && list.Count > 0)
            {
                planet = list[list.Count - 1];
                
                list.RemoveAt(list.Count - 1);
                
                //UnityEngine.Debug.Log("returning from pool!");
            }
            else
            {
                planet = new Planet();
            }
            
            return planet;
        }
        
        public void ReturnPlanet(Planet planet)
        {
            int height = planet.Height;
            
            planet.Recycle();
            
            List<Planet> list;
            
            if (!planetsPool.TryGetValue(height, out list))
            {
                list = new List<Planet>();            
                planetsPool.Add(height, list);
            }
            
            planetsPool[height].Add(planet);
        }
    }
}

