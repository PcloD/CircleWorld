using System;

namespace UniverseEngine
{
    public class Avatar : UniverseObject
    {
        public AvatarInput input = new AvatarInput();
        
        public float jumpSpeed = 7.0f;
        public float walkSpeed = 3.0f;
        
        protected override void OnUpdate()
        {
            if (CanWalk())
                Walk(input.walkDirection);

            if (input.jump)
                if (CanJump())
                    Jump();
            
            input.Reset();
        }
        
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

