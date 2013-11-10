using System;

namespace UniverseEngine
{
    public class ShipInput
    {
        public float moveDirection;
        public float rotateDirection;
        
        public void Reset()
        {
            moveDirection = 0;
            rotateDirection = 0;
        }
    }
}