using UnityEngine;
using System.Collections;

namespace Universe
{
    public class TilemapObject
    {
        public TilemapCircle tilemapCircle;
    
        protected bool useGravity = true;
     
        protected Vector2 position;
        protected float scale;
        protected float rotation; //radians
        
        protected Vector2 size = new Vector2(1, 1);
        protected Vector2 velocity;
    
        protected TileHitFlags hitFlags;
        
        protected float distanceInTilemapCircle;
        protected float angleInTilemapCirclePosition;
        
        private ITilemapObjectListener listener;
        
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
        
        public ITilemapObjectListener Listener
        {
            get { return listener; }
            set { this.listener = value; }
        }
        
        public void Init(Vector2 size, TilemapCircle tilemapCircle, Vector2 position)
        {
            this.size = size;
            
            SwitchToTilemapCircle(tilemapCircle, position);
        }
        
        public void SwitchToTilemapCircle(TilemapCircle tilemapCircle, Vector2 position)
        {
            this.tilemapCircle = tilemapCircle;
            this.position = position;
            this.scale = tilemapCircle.GetScaleFromPosition(position);
            this.rotation = tilemapCircle.GetAngleFromPosition(position);
            
            distanceInTilemapCircle = tilemapCircle.GetDistanceFromPosition(position);
            angleInTilemapCirclePosition = tilemapCircle.GetAngleFromPosition(position);
            
            if (listener != null)
                listener.OnTilemapCircleChanged();
        }
        
        public void UpdatePosition(float deltaTime)
        {
            if (tilemapCircle != null)
            {
                position = tilemapCircle.GetPositionFromDistanceAndAngle(distanceInTilemapCircle, angleInTilemapCirclePosition);
                rotation = tilemapCircle.GetAngleFromPosition(position);
            }
            
            scale = tilemapCircle.GetScaleFromPosition(position);
            Vector2 normal = tilemapCircle.GetNormalFromPosition(position); //doesn't change with vertical position
            Vector2 tangent = tilemapCircle.GetTangentFromPosition(position); //doesn't change with vertical position
    
            if (tilemapCircle is Planet && useGravity)
                velocity.y -= ((Planet) tilemapCircle).Gravity * deltaTime;
    
            Vector2 delta = velocity * deltaTime * scale;
    
            TileHitInfo hitInfo;
    
            hitFlags = TileHitFlags.None;
    
            if (delta.y > 0)
            {
                //Check against ceiling
                if (tilemapCircle.RaycastSquare(
                    position + normal * (size.y * 0.5f * scale), 
                    size.x * scale,
                    TileDirection.Up, 
                    delta.y + (size.y * 0.5f * scale), 
                    out hitInfo))
                {
                    delta.y = -(hitInfo.hitDistance - (size.y * 0.5f * scale));
                    velocity.y = 0.0f;
                    hitFlags |= TileHitFlags.Up;
                }
            }
            else if (delta.y < 0)
            {
                //Check against floor
                if (tilemapCircle.RaycastSquare(
                    position + normal * (size.y * 0.5f * scale), 
                    size.x * scale,
                    TileDirection.Down, 
                    -delta.y + (size.y * 0.5f * scale), 
                    out hitInfo))
                {
                    delta.y = -(hitInfo.hitDistance - (size.y * 0.5f * scale));
                    velocity.y = 0.0f;
                    hitFlags |= TileHitFlags.Down;
                }
            }
    
            if (delta.y != 0)
            {
                position += normal * delta.y;
                scale = tilemapCircle.GetScaleFromPosition(position);
            }
    
            if (delta.x > 0)
            {
                //Check against right wall
                if (tilemapCircle.RaycastSquare(
                    position + normal * (size.y * 0.5f * scale), 
                    size.y * scale,
                    TileDirection.Right, 
                    delta.x + (size.x * 0.5f * scale), 
                    out hitInfo))
                {
                    delta.x = (hitInfo.hitDistance - (size.x * 0.5f * scale));
                    velocity.x = 0.0f;
                    hitFlags |= TileHitFlags.Right;
                }
            }
            else if (delta.x < 0)
            {
                //Check against left wall
                if (tilemapCircle.RaycastSquare(
                    position + normal * (size.y * 0.5f * scale), 
                    size.y * scale,
                    TileDirection.Left, 
                    -delta.x + (size.x * 0.5f * scale), 
                    out hitInfo))
                {
                    delta.x = -(hitInfo.hitDistance - (size.x * 0.5f * scale));
                    velocity.x = 0.0f;
                    hitFlags |= TileHitFlags.Left;
                }
            }
    
            if (delta.x != 0)
            {
                position += tangent * delta.x;
                normal = tilemapCircle.GetNormalFromPosition(position);
            }
    
            rotation = tilemapCircle.GetAngleFromPosition(position);
            
            if (tilemapCircle != null)
            {
                distanceInTilemapCircle = tilemapCircle.GetDistanceFromPosition(position);
                angleInTilemapCirclePosition = tilemapCircle.GetAngleFromPosition(position);
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
