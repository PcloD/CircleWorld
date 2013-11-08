using UnityEngine;
using System.Collections.Generic;
using UniverseEngine;

public class UniverseViewFactory : MonoBehaviour
{
    public GameObject avatarPrefab;
    public GameObject shipPrefab;
    public GameObject planetPrefab;
    
    private Dictionary<int, List<PlanetView>> planetsPool = new Dictionary<int, List<PlanetView>>();
    
    public AvatarView GetAvatar()
    {
        return ((GameObject) GameObject.Instantiate(avatarPrefab)).GetComponent<AvatarView>();
    }
    
    public ShipView GetShip()
    {
        return ((GameObject) GameObject.Instantiate(shipPrefab)).GetComponent<ShipView>();
    }
    
    public PlanetView GetPlanet(int height)
    {
        PlanetView planet = null;
        
        List<PlanetView> list;
        
        if (planetsPool.TryGetValue(height, out list) && list.Count > 0)
        {
            planet = list[list.Count - 1];
            
            list.RemoveAt(list.Count - 1);
        }
        else
        {
            planet = ((GameObject) GameObject.Instantiate(planetPrefab)).GetComponent<PlanetView>();
        }
        
        return planet;
    }
    
    public void ReturnPlanet(PlanetView planet)
    {
        int height = planet.TilemapCircle.Height;
        
        planet.Recycle();
        
        List<PlanetView> list;
        
        if (!planetsPool.TryGetValue(height, out list))
        {
            list = new List<PlanetView>();            
            planetsPool.Add(height, list);
        }
        
        planetsPool[height].Add(planet);
    }
}

