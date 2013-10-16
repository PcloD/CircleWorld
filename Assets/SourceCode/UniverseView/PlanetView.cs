using UnityEngine;
using System.Collections;
using UniverseEngine;

public class PlanetView : TilemapCircleView
{
    private UniverseView universeView;
    private Planet planet;
    
    public UniverseView UniverseView
    {
        get { return universeView; }
    }
    
    public Planet Planet
    {
        get { return planet; }
    }
    
    public void InitPlanet(Planet planet, UniverseView universeView)
    {
        this.planet = planet;
        this.universeView = universeView;
        
        Init(planet);
        
        gameObject.SetActive(true);
    }
    
    public override void Recycle()
    {
        universeView = null;
        planet = null;
        
        base.Recycle();
    }
}
