using UnityEngine;
using System;

namespace UniverseEngine
{
    public class Ship : UniverseObject
    {
        public ShipInput input = new ShipInput();
        
        private float movementSpeedMax = 50.0f;
        private float movementAcceleration = 100.0f;
        private float movementFriction = 200.0f;
        
        private float rotationSpeedMax = 135.0f * Mathf.Deg2Rad;
        private float rotationAcceleration = 360.0f * Mathf.Deg2Rad;
        private float rotationFriction = 360.0f * Mathf.Deg2Rad;
                
        protected override void OnUpdate(float deltaTime)
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
            
            input.Reset();
        }
    }
}

