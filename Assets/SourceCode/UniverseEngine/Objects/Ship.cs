using UnityEngine;
using System;

namespace UniverseEngine
{
    public class Ship : UniverseObject
    {
        public ShipInput input = new ShipInput();
        
        private float movementSpeedMax = 100.0f;
        private float movementAcceleration = 100.0f;
        private float movementFriction = 200.0f;
        
        private float rotationSpeedMax = 135.0f * Mathf.Deg2Rad;
        private float rotationAcceleration = 360.0f * Mathf.Deg2Rad;
        private float rotationFriction = 360.0f * Mathf.Deg2Rad;
        
        public Ship()
        {
            useGravity = false;
        }
                
        protected override void OnUpdate(float deltaTime)
        {
            if (GameLogic.Instace.State == GameLogicState.PlayingShip)
            {
                if (input.rotateDirection != 0)
                    rotationVelocity += input.rotateDirection * rotationAcceleration * deltaTime;
                else
                    rotationVelocity -= Mathf.Sign(rotationVelocity) * Mathf.Clamp(rotationFriction * deltaTime, 0, Mathf.Abs(rotationVelocity));
                
                rotationVelocity = Mathf.Clamp(rotationVelocity, -rotationSpeedMax, rotationSpeedMax);
                
                float currentSpeed = velocity.magnitude;
                
                if (input.moveDirection > 0)
                    currentSpeed += input.moveDirection * movementAcceleration * deltaTime;
                else
                    currentSpeed -= Mathf.Sign(currentSpeed) * Mathf.Clamp(movementFriction * deltaTime, 0, Mathf.Abs(currentSpeed));
                
                currentSpeed = Mathf.Clamp(currentSpeed, 0, movementSpeedMax);
                           
                velocity.x = Mathf.Sin(Rotation) * currentSpeed;
                velocity.y = Mathf.Cos(Rotation) * currentSpeed;
            }
            else if (GameLogic.Instace.State == GameLogicState.PlayingAvatar)
            {
                //Orbit planet!
                if (parent != null)
                {
                    float orbitDistance = parent.GetDistanceFromTileY(parent.Height + 7);
                    
                    if (distanceInTilemapCircle > orbitDistance + 1.0f)
                        velocity.y -= movementAcceleration * Time.deltaTime;
                    else if (distanceInTilemapCircle < orbitDistance - 1.0f)
                        velocity.y += movementAcceleration * Time.deltaTime;
                    else
                        velocity.y -= Mathf.Sign(velocity.y) * Mathf.Clamp(movementFriction * deltaTime, 0, Mathf.Abs(velocity.y));
                    
                    velocity.y = Mathf.Clamp(velocity.y, 0, movementSpeedMax * 0.1f);
                    
                    velocity.x = movementSpeedMax * 0.05f;
                    
                    float orbitRotationDiff = (parent.GetAngleFromPosition(position) + Mathf.PI * 0.5f) - rotation;
                    
                    if (orbitRotationDiff > Mathf.PI)
                        orbitRotationDiff -= Mathf.PI * 2.0f;
                    else if (orbitRotationDiff < -Mathf.PI)
                        orbitRotationDiff += Mathf.PI * 2.0f;
                    
                    if (orbitRotationDiff < 0)
                        rotationVelocity -= rotationAcceleration * Time.deltaTime;
                    else if (orbitRotationDiff > 0)
                        rotationVelocity += rotationAcceleration * Time.deltaTime;
                    else
                        rotationVelocity -= Mathf.Sign(rotationVelocity) * Mathf.Clamp(rotationFriction * deltaTime, 0, Mathf.Abs(rotationVelocity));
                    
                    rotationVelocity = Mathf.Clamp(rotationVelocity, -rotationSpeedMax * 0.1f, rotationSpeedMax * 0.1f);
                }
            }
            
            input.Reset();
        }
        
        public void BeamDownAvatar(Avatar avatar, Planet planet)
        {
            rotationVelocity = 0.0f;
            velocity = Vector2.zero;
            
            SetParent(planet, FollowParentParameters.None, position, rotation);
            
            avatar.TravelToPlanet(planet);
        }
        
        public void BeamUpAvatar(Avatar avatar)
        {
            SetParent(null, FollowParentParameters.None, position, rotation);
            
            avatar.BoardShip(this);
        }
    }
}

