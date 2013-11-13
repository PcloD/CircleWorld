using UnityEngine;
using UniverseEngine;
using System;

public class PlanetArrow : MonoBehaviour
{
    [HideInInspector]
    [System.NonSerialized]
    public Transform trans;
    
    public SpriteView planet;

    private GameObject go;
    private bool visible = true;
    
    public void Awake()
    {
        trans = transform;
        go = gameObject;
    }
    
    public void UpdatePlanet(Thing thing)
    {
        PlanetType planetType;
        
        if (thing.type == (ushort) ThingType.Sun)
            planetType = PlanetTypes.GetPlanetType((byte) (Math.Abs(thing.seed % 2) + 4)); //suns!
        else
            planetType = PlanetTypes.GetPlanetType((byte) (Math.Abs(thing.seed % 4))); //planets!
        
        planet.UpdateSprite("Planets", planetType.planetSprite.Id);
    }

    public void Hide()
    {
        if (visible)
        {
            visible = false;
            go.SetActive(false);
        }
    }

    public void Show()
    {
        if (!visible)
        {
            visible = true;
            go.SetActive(true);
        }
    }
}


