using UnityEngine;
using System.Collections;
using UniverseEngine;

public class AvatarView : UniverseObjectView
{
    public AvatarInput avatarInput;
    
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
        UniverseEngine.Avatar avatar = (UniverseEngine.Avatar) universeObject;
        
        if (avatar.CanWalk())
            avatar.Walk(avatarInput.walkDirection);

        if (avatarInput.jump)
            if (avatar.CanJump())
                avatar.Jump();
        
        avatarInput.ResetInput();
    }




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
