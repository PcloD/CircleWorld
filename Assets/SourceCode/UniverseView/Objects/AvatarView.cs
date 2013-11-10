using UnityEngine;
using System.Collections;
using UniverseEngine;

public class AvatarView : UniverseObjectView
{
    public override void OnDrawGizmos ()
    {
        float sizeY = 1.05f;
        float sizeX = 0.75f;
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * sizeY);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + transform.up * sizeY * 0.5f - transform.right * sizeX * 0.5f, transform.position + transform.up * sizeY * 0.5f + transform.right * sizeX * 0.5f);
    }
}
