using UnityEngine;
using UniverseEngine;
using System;

public class PlanetArrow : MonoBehaviour
{
    [HideInInspector]
    [System.NonSerialized]
    public Transform trans;
    
    public SpriteView planet;
    
    public void Awake()
    {
        trans = transform;
    }
    
    public void UpdatePlanet(Thing thing)
    {
        PlanetType planetType;
        
        if (thing.type == (ushort) ThingType.Sun)
            planetType = PlanetTypes.GetPlanetType((byte) (Math.Abs(thing.seed % 2) + 4)); //suns!
        else
            planetType = PlanetTypes.GetPlanetType((byte) (Math.Abs(thing.seed % 4))); //planets!
        
        planet.UpdateMesh("Planets", planetType.planetSprite.Id);
    }
}


