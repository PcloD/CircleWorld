using UnityEngine;
using System.Collections;
using UniverseEngine;

public class ShipView : UniverseObjectView
{
    public override void OnParentChanged (TilemapCircle parent)
    {
        base.OnParentChanged (parent);
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
