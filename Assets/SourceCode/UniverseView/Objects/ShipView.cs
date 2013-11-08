using UnityEngine;
using System.Collections;
using UniverseEngine;

public class ShipView : UniverseObjectView
{
    public ShipInput shipInput;
    
    public override void OnUniverseObjectUpdated(float deltaTime)
    {
        UpdatePosition();
    }
    
    public override void OnParentChanged (TilemapCircle parent)
    {
        base.OnParentChanged (parent);
    }
    
    
    public void ProcessInput()
    {
        UniverseEngine.Ship ship = (UniverseEngine.Ship) universeObject;
        
        ship.Move(shipInput.moveDirection, shipInput.rotateDirection);

        shipInput.ResetInput();
    }

    public override void OnDrawGizmos ()
    {
        float sizeY = 1.0f;
        float sizeX = 1.0f;
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * sizeY);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position - transform.right * sizeX * 0.5f, transform.position + transform.right * sizeX * 0.5f);
    }
}
