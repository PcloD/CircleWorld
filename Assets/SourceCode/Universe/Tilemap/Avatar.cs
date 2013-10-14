using System;

namespace Universe
{
    public class Avatar : TilemapObject
    {
        public float jumpSpeed = 7.0f;
        public float walkSpeed = 3.0f;
        
        public bool CanWalk()
        {
            return true;
        }
        
        public void Walk(float direction)
        {
            velocity.x = direction * walkSpeed;
        }
        
        public bool CanJump()
        {
            return (hitFlags & TileHitFlags.Down) != 0;
        }
        
        public void Jump()
        {
            velocity.y = jumpSpeed;
        }
    }
}

