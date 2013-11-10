using UnityEngine;
using System;

namespace UniverseEngine
{
    public class Ship : UniverseObject
    {
        public ShipInput input = new ShipInput();
        
        public float movementSpeed = 50.0f;
        public float rotationSpeed = 90.0f;
                
        protected override void OnUpdate ()
        {
            rotationVelocity = input.rotateDirection * rotationSpeed * Mathf.Deg2Rad;
            velocity.x = Mathf.Sin(Rotation) * movementSpeed * input.moveDirection;
            velocity.y = Mathf.Cos(Rotation) * movementSpeed * input.moveDirection;
            
            input.Reset();
        }
    }
}

