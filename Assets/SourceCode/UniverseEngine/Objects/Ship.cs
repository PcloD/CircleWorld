using UnityEngine;
using System;

namespace UniverseEngine
{
    public class Ship : UniverseObject
    {
        public float movementSpeed = 50.0f;
        public float rotationSpeed = 90.0f;
        
        public void Move(float moveDirection, float rotateDirection)
        {
            rotationVelocity = rotateDirection * rotationSpeed * Mathf.Deg2Rad;
            velocity.x = Mathf.Sin(Rotation) * movementSpeed * moveDirection;
            velocity.y = Mathf.Cos(Rotation) * movementSpeed * moveDirection;
        }
    }
}

