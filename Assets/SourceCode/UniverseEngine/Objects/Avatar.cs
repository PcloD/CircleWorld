using System;
using UnityEngine;

namespace UniverseEngine
{
    public class Avatar : UniverseObject
    {
        public AvatarInput input = new AvatarInput();
        
        private float jumpSpeed = 7.0f;
        private float walkSpeedMax = 3.0f;
        private float walkAcceleration = 10.0f;
        private float walkFriction = 10.0f;
        
        protected override void OnUpdate(float deltaTime)
        {
            if (CanWalk())
            {
                if (input.walkDirection != 0)
                    velocity.x += input.walkDirection * walkAcceleration * deltaTime;
                else
                    velocity.x -= Mathf.Sign(velocity.x) * Mathf.Clamp(walkFriction * deltaTime, 0, Mathf.Abs(velocity.x));
                    
                velocity.x = Mathf.Clamp(velocity.x, -walkSpeedMax, walkSpeedMax);
            }

            if (input.jump && CanJump())
                velocity.y = jumpSpeed;
            
            input.Reset();
        }
        
        public bool CanWalk()
        {
            return true;
        }
                
        public bool CanJump()
        {
            return (hitFlags & TileHitFlags.Down) != 0;
        }
    }
}

