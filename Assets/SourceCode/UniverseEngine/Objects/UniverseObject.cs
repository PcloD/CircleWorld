using UnityEngine;
using System.Collections;

namespace UniverseEngine
{
    public class UniverseObject
    {
        public TilemapCircle parent;
    
        protected bool useGravity = true;
     
        protected Vector2 position;
        protected float scale;
        protected float rotation; //radians
        
        protected Vector2 size = new Vector2(1, 1);
        
        protected Vector2 velocity;
        protected float rotationVelocity;
    
        protected TileHitFlags hitFlags;
        
        protected float distanceInTilemapCircle;
        protected float angleInTilemapCirclePosition;
        
        private IUniverseObjectListener listener;
        
        public Vector2 Position
        {
            get { return position; }
        }
        
        public float Scale
        {
            get { return scale; }
        }
        
        public float Rotation
        {
            get { return rotation; }
        }
        
        public Vector2 Size
        {
            get { return size; }
        }
        
        public Vector2 Velocity
        {
            get { return velocity; }
            set { this.velocity = value; }
        }
        
        public TileHitFlags HitFlags
        {
            get { return hitFlags; }
        }
        
        public IUniverseObjectListener Listener
        {
            get { return listener; }
            set { this.listener = value; }
        }
        
        public void Init(Vector2 size, TilemapCircle parent, Vector2 position)
        {
            this.size = size;
            
            SetParent(parent, position);
        }
        
        public void SetParent(TilemapCircle parent, Vector2 position)
        {
            this.parent = parent;
            this.position = position;
            
            if (parent != null)
            {
                this.scale = parent.GetScaleFromPosition(position);
                this.rotation = parent.GetAngleFromPosition(position);
                
                distanceInTilemapCircle = parent.GetDistanceFromPosition(position);
                angleInTilemapCirclePosition = parent.GetAngleFromPosition(position);
            }
            else
            {
                this.scale = 1.0f;
                this.rotation = 0.0f;
            }
            
            if (listener != null)
                listener.OnParentChanged(parent);
        }
        
        public void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
            
            UpdatePosition(deltaTime);
            
            if (listener != null)
                listener.OnUniverseObjectUpdated(deltaTime);
        }
        
        protected virtual void OnUpdate(float deltaTime)
        {
            
        }
        
        protected void UpdatePosition(float deltaTime)
        {
            Vector2 normal;
            Vector2 tangent;
            
            Vector2 deltaPosition;
            float deltaRotation;
                
            if (parent != null)
            {
                position = parent.GetPositionFromDistanceAndAngle(distanceInTilemapCircle, angleInTilemapCirclePosition);
                rotation = parent.GetAngleFromPosition(position);
                
                scale = parent.GetScaleFromPosition(position);
                
                if (parent is Planet && useGravity)
                    velocity.y -= ((Planet) parent).Gravity * deltaTime;
                
                normal = parent.GetNormalFromPosition(position); //doesn't change with vertical position
                tangent = parent.GetTangentFromPosition(position); //doesn't change with vertical position
                
                deltaPosition = velocity * deltaTime * scale;
                deltaRotation = 0.0f;
            }
            else
            {
                normal = Vector2.up;
                tangent = Vector2.right;
                
                deltaPosition = velocity * deltaTime;
                deltaRotation = rotationVelocity * deltaTime;
            }
            
            hitFlags = TileHitFlags.None;
            
            if (parent != null)
            {
                TileHitInfo hitInfo;
        
                if (deltaPosition.y > 0)
                {
                    //Check against ceiling
                    if (parent.RaycastSquare(
                        position + normal * (size.y * 0.5f * scale), 
                        size.x * scale,
                        TileDirection.Up, 
                        deltaPosition.y + (size.y * 0.5f * scale), 
                        out hitInfo))
                    {
                        deltaPosition.y = -(hitInfo.hitDistance - (size.y * 0.5f * scale));
                        velocity.y = 0.0f;
                        hitFlags |= TileHitFlags.Up;
                    }
                }
                else if (deltaPosition.y < 0)
                {
                    //Check against floor
                    if (parent.RaycastSquare(
                        position + normal * (size.y * 0.5f * scale), 
                        size.x * scale,
                        TileDirection.Down, 
                        -deltaPosition.y + (size.y * 0.5f * scale), 
                        out hitInfo))
                    {
                        deltaPosition.y = -(hitInfo.hitDistance - (size.y * 0.5f * scale));
                        velocity.y = 0.0f;
                        hitFlags |= TileHitFlags.Down;
                    }
                }
            }
    
            if (deltaPosition.y != 0)
            {
                position += normal * deltaPosition.y;
                if (parent != null)
                    scale = parent.GetScaleFromPosition(position);
            }
            
            if (parent != null)
            {
                TileHitInfo hitInfo;
                
                if (deltaPosition.x > 0)
                {
                    //Check against right wall
                    if (parent.RaycastSquare(
                        position + normal * (size.y * 0.5f * scale), 
                        size.y * scale,
                        TileDirection.Right, 
                        deltaPosition.x + (size.x * 0.5f * scale), 
                        out hitInfo))
                    {
                        deltaPosition.x = (hitInfo.hitDistance - (size.x * 0.5f * scale));
                        velocity.x = 0.0f;
                        hitFlags |= TileHitFlags.Right;
                    }
                }
                else if (deltaPosition.x < 0)
                {
                    //Check against left wall
                    if (parent.RaycastSquare(
                        position + normal * (size.y * 0.5f * scale), 
                        size.y * scale,
                        TileDirection.Left, 
                        -deltaPosition.x + (size.x * 0.5f * scale), 
                        out hitInfo))
                    {
                        deltaPosition.x = -(hitInfo.hitDistance - (size.x * 0.5f * scale));
                        velocity.x = 0.0f;
                        hitFlags |= TileHitFlags.Left;
                    }
                }
            }
    
            if (deltaPosition.x != 0)
            {
                position += tangent * deltaPosition.x;
                if (parent != null)
                    normal = parent.GetNormalFromPosition(position);
            }
    
            if (parent != null)
            {
                rotation = parent.GetAngleFromPosition(position);
                
                distanceInTilemapCircle = parent.GetDistanceFromPosition(position);
                angleInTilemapCirclePosition = parent.GetAngleFromPosition(position);
            }
            else
            {
                rotation += deltaRotation;
            }
        }
     
        /*
        public bool MoveTo(Vector2 position)
        {
            if (CanMoveTo(position))
            {
                this.position = position;
                return true;
            }
    
            return false;
        }
    
        public bool CanMoveTo(Vector2 position)
        {
            float scale = tilemapCircle.GetScaleFromPosition(position);
    
            int tileX, tileY;
    
            Vector2 right = transform.right;
            Vector2 up = transform.up;
    
            position += up * 0.05f;
    
            for (int x = -1; x <= 1; x++)
            {
                for (int y = 0; y <= 2; y++)
                {
                    Vector2 pos = position + 
                        right * (size.x * 0.9f * x * 0.5f * scale) +
                            up * ((size.y * 0.9f / 2) * y * scale);
    
                    if (tilemapCircle.GetTileCoordinatesFromPosition(pos, out tileX, out tileY))
                        if (tilemapRicle.GetTile(tileX, tileY) != 0)
                            return false;
                }
            }
    
            return true;
        }
        */
    }
}
