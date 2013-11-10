using System;

namespace UniverseEngine
{
    public class AvatarInput
    {
        public float walkDirection;
        public bool jump;
        
        public void Reset()
        {
            walkDirection = 0.0f;
            jump = false;
        }
    }
}