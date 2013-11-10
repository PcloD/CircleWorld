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
        
        private Ship onShip;
        
        protected override void OnUpdate(float deltaTime)
        {
            if (onShip == null)
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
            }
            else
            {
                velocity = Vector2.zero;
                position = onShip.Position;
            }
            
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
        
        public void BoardShip(Ship ship)
        {
            this.onShip = ship;
            this.scale = 1.0f;
            
            SetParent(
                null,
                FollowParentParameters.None,
                ship.Position,
                rotation);
            
            Visible = false;
        }
        
        public void TravelToPlanet(Planet planet)
        {
            if (this.onShip != null)
            {
                //Set position closest to the ship
                int landTileX;
                int landTileY;
                planet.GetTileCoordinatesFromPosition(onShip.Position, out landTileX, out landTileY);
                landTileY = planet.Height;
                
                SetParent(
                    planet,
                    FollowParentParameters.Default,
                    planet.GetPositionFromTileCoordinate(landTileX, landTileY),
                    0.0f
                );
                
                //Leave ship
                this.onShip = null;
            }
            else
            {
                SetParent(
                    planet,
                    FollowParentParameters.Default,
                    planet.GetPositionFromTileCoordinate(0, planet.Height),
                    0.0f
                );
            }
            
            Visible = true;
        }
    }
}

