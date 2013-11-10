using UnityEngine;
using System.Collections.Generic;
using UniverseEngine;

public class ShipView : UniverseObjectView
{
    public override void OnDrawGizmos ()
    {
        float sizeY = 1.0f;
        float sizeX = 1.0f;
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * sizeY);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position - transform.right * sizeX * 0.5f, transform.position + transform.right * sizeX * 0.5f);
    }
    
    private List<ushort> closeThings;
    
    public override void OnUniverseObjectUpdated (float deltaTime)
    {
        base.OnUniverseObjectUpdated (deltaTime);
        
        if (GameLogic.Instace.State == GameLogicState.PlayingShip)
        {
            closeThings = universeView.Universe.FindClosestRenderedThings(universeObject.Position, 100.0f, closeThings);
            
            for (int i = 0; i < closeThings.Count && i < universeView.MaxActivePlanetViews; i++)
                universeView.GetPlanetView(closeThings[i]);
        }
    }
}
