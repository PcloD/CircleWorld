using UnityEngine;
using System.Collections.Generic;

namespace UniverseEngine
{
    public class TilemapCircle
    {
        public const float TILE_SIZE = 0.5f; 
        public const float TILE_SIZE_INV = 1.0f / TILE_SIZE;
    
        protected int seed;
        protected int height;
        protected int width;
    
        protected byte[] tiles;
    
        protected Vector2[] circleNormals;
        protected float[] circleHeights;
    
        //Used when fiding tileY positions!
        private float height0;
        private float k;
        private float logk;
        
        protected ITilemapCircleListener listener;
        
        protected Vector2 position;
        protected float rotation;
        
        public int Height
        {
            get { return height; }
        }
        
        public int Width
        {
            get { return width; }
        }
        
        public int Seed
        {
            get { return seed; }
        }
        
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        
        public Vector2[] CircleNormals
        {
            get { return circleNormals; }
        }
        
        public float[] CircleHeights
        {
            get { return circleHeights; }
        }
        
        public ITilemapCircleListener Listener
        {
            get { return listener; }
            set { this.listener = value; }
        }
        
        public void Init(int seed, int height)
        {
            if (height < 5)
                height = 5;
            
            this.seed = seed;
            this.height = height;
            
            UpdateData();
            
            UpdateTiles();
        }
        
        protected virtual void UpdateTiles()
        {
            //TODO: Override!
        }
    
        private void UpdateData()
        {
            width = (((int)((float)height * Mathf.PI * 2.0f)) / 4) * 4;
    
            if (circleNormals == null || circleNormals.Length != width)
                circleNormals = new Vector2[width];
    
            if (circleHeights == null || circleHeights.Length != height + 1)
                circleHeights = new float[height + 1];
    
            if (tiles == null || tiles.Length != width * height)
                tiles = new byte[width * height];
            
            float angleStep = ((2.0f * Mathf.PI) / width);
    
            for (int i = 0; i < width; i++)
            {
                float angle = i * angleStep;
                circleNormals[i].x = Mathf.Sin(angle);
                circleNormals[i].y = Mathf.Cos(angle);
            }
    
            height0 = (height - 1) * TILE_SIZE;
            k = -((width / (Mathf.PI * 2.0f))) / (1 - (width / (Mathf.PI * 2.0f)));
            logk = Mathf.Log(k);
    
            circleHeights[0] = height0;
    
            for (int i = 1; i <= height; i++)
            {
                float r1 = circleHeights[i - 1];
    
                //float r2 = ((-r1 * width) / (Mathf.PI * 2.0f)) / (1 - (width / (Mathf.PI * 2.0f)));
                float r2 = r1 * k;
    
                circleHeights[i] = r2;
            }
        }
    
        public byte GetTile(int tileX, int tileY)
        {
            return tiles[tileX + tileY * width];
        }
    
        public void SetTile(int tileX, int tileY, byte tile)
        {
            if (tiles[tileX + tileY * width] != tile)
            {
                tiles[tileX + tileY * width] = tile;
                if (listener != null)
                    listener.OnTilemapTileChanged(tileX, tileY);
            }
        }
    
        public int GetTileYFromDistance(float distance)
        {
            //This was taken from wolfram-alpha, by solving the radius relationship function
            //Original function: http://www.wolframalpha.com/input/?i=g%280%29%3Dk%2C+g%28n%2B1%29%3Dl+*+g%28n%29
            //Solution: http://www.wolframalpha.com/input/?i=y+%3D+k+*+l%CB%86x+find+x (we use the solution over reals with y > 0)
    
            //int tileY = (int) (Mathf.Log (distance / height0) / Mathf.Log (k));
            int tileY = (int) (Mathf.Log (distance / height0) / logk);
    
            return tileY;
        }
    
        public float GetDistanceFromTileY(int tileY)
        {
            return height0 * Mathf.Pow(k, (float) tileY);
        }
    
        public float GetDistanceFromPosition(Vector2 position)
        {
            float dx = position.x - this.position.x;
            float dy = position.y - this.position.y;
            
            return Mathf.Sqrt(dx * dx + dy * dy);
        }
        
        public int GetTileXFromAngle(float angle)
        {
            int tileX = Mathf.FloorToInt((angle / (Mathf.PI * 2.0f)) * width);
    
            tileX = tileX % width;
            if (tileX < 0)
                tileX += width;
    
            return tileX;
        }
    
        public Vector2 GetPositionFromTileCoordinate(int tileX, int tileY)
        {
            return position + GetNormalFromTileX(tileX) * GetDistanceFromTileY(tileY);
        }
        
        public Vector2 GetPositionFromDistanceAndAngle(float distance, float angle)
        {
            return position + GetNormalFromAngle(angle) * distance;
        }
    
        public bool GetTileCoordinatesFromPosition(Vector2 position, out int tileX, out int tileY)
        {
            float dx = position.x - this.position.x;
            float dy = position.y - this.position.y;
    
            float distance = Mathf.Sqrt(dx * dx + dy * dy);
            float angle = -Mathf.Atan2(dy, dx) + Mathf.PI * 0.5f;
    
            tileY = GetTileYFromDistance(distance);
            tileX = GetTileXFromAngle(angle);
    
            if (tileY >= height || tileY < 0)
                return false;
    
            return true;
        }
    
        public float GetScaleFromPosition(Vector2 position)
        {
            float dx = position.x - this.position.x;
            float dy = position.y - this.position.y;
            float distance = Mathf.Sqrt(dx * dx + dy * dy);
    
            float scale = Mathf.Clamp(
                (distance * 2.0f * Mathf.PI) / width,
                (circleHeights[0] * 2.0f * Mathf.PI) / width,
                (circleHeights[circleHeights.Length - 1] * 2.0f * Mathf.PI) / width) * TILE_SIZE_INV;
    
            return scale;
        }
    
        public Vector2 GetNormalFromPosition(Vector2 position)
        {
            float dx = position.x - this.position.x;
            float dy = position.y - this.position.y;
            float distance = Mathf.Sqrt(dx * dx + dy * dy);
    
            return new Vector2(dx / distance, dy / distance);
        }
        
        public Vector2 GetNormalFromAngle(float angle)
        {
            return new Vector2(
                Mathf.Sin(angle), 
                Mathf.Cos(angle)
            );
        }
    
        public float GetAngleFromPosition(Vector2 position)
        {
            float dx = position.x - this.position.x;
            float dy = position.y - this.position.y;
    
            float angle = -Mathf.Atan2(dy, dx) + Mathf.PI * 0.5f;
    
            return angle;
        }
        
        public Vector2 GetNormalFromTileX(int tileX)
        {
            tileX = tileX % width;
    
            return circleNormals[tileX];
        }
    
        public Vector2 GetTangentFromPosition(Vector2 position)
        {
            Vector2 normal = GetNormalFromPosition(position);
    
            return new Vector2(normal.y, -normal.x);
        }
    
        public Vector2 GetTangentFromTileCoordinate(int tileX, int tileY)
        {
            Vector2 normal = GetNormalFromTileX(tileX);
    
            return new Vector2(normal.y, -normal.x);
        }
    
        public bool RaycastSquare(Vector2 origin, float size, TileDirection direction, float len, out TileHitInfo hitInfo)
        {
            size *= 0.95f;
    
            hitInfo = new TileHitInfo();
    
            int iterations = Mathf.Max(Mathf.CeilToInt(size / TILE_SIZE), 1);
    
            Vector2 from = origin - GetTanget(origin, direction) * (size * 0.5f);
            Vector2 step = GetTanget(origin, direction) * (size / iterations);
    
            bool hitAny = false;
            TileHitInfo localHitInfo;
    
            for (int i = 0; i <= iterations; i++)
            {
                if (Raycast(from, direction, len, out localHitInfo))
                {
                    if (!hitAny)
                    {
                        hitAny = true;
                        hitInfo = localHitInfo;
                    }
                    else if (localHitInfo.hitDistance < hitInfo.hitDistance)
                    {
                        hitInfo = localHitInfo;
                    }
                }
    
                from += step;
            }
    
            return hitAny;
        }
    
        public Vector2 GetDirection(Vector2 origin, TileDirection direction)
        {
            switch(direction)
            {
                case TileDirection.Down:
                    return -GetNormalFromPosition(origin);
                
                case TileDirection.Up:
                    return GetNormalFromPosition(origin);
    
                case TileDirection.Right:
                    return GetTangentFromPosition(origin);
    
                case TileDirection.Left:
                    return -GetTangentFromPosition(origin);
            }
    
            return Vector2.zero;
        }
    
        
        public Vector2 GetTanget(Vector2 origin, TileDirection direction)
        {
            switch(direction)
            {
                case TileDirection.Down:
                    return GetTangentFromPosition(origin);
    
                case TileDirection.Up:
                    return GetTangentFromPosition(origin);
    
                case TileDirection.Right:
                    return GetNormalFromPosition(origin);
    
                case TileDirection.Left:
                    return GetNormalFromPosition(origin);
            }
    
            return Vector2.zero;
        }
        public bool Raycast(Vector2 origin, TileDirection direction, float len, out TileHitInfo hitInfo)
        {
            hitInfo = new TileHitInfo();
    
            float dx = origin.x - position.x;
            float dy = origin.y - position.y;
            float originDistance = Mathf.Sqrt(dx * dx + dy * dy);
    
            Vector2 target;
            float targetdx;
            float targetdy;
            float targetDistance;
            float tangentDistance;
    
            float segmentSize;
    
            if (originDistance < 0.001f)
                originDistance = 0.001f;
    
            float originMapAngle = -Mathf.Atan2(dy, dx) + Mathf.PI * 0.5f;
    
            while (originMapAngle > Mathf.PI * 2.0f)
                originMapAngle -= Mathf.PI * 2.0f;
    
            while (originMapAngle < 0.0f)
                originMapAngle += Mathf.PI * 2.0f;
    
            Vector2 originNormal = new Vector2(dx / originDistance, dy / originDistance);
            Vector2 originTangent = new Vector2(originNormal.y, -originNormal.x);
    
            if (direction == TileDirection.Right)
            {
                target = origin + originTangent * len;
                targetdx = target.x - position.x;
                targetdy = target.y - position.y;
                targetDistance = Mathf.Sqrt(targetdx * targetdx + targetdy * targetdy);
    
                if (originDistance > circleHeights[circleHeights.Length - 1])
                {
                    //Origin point outside, not hit!
                    return false;
                }
    
                for (int i = 1; i < circleHeights.Length; i++)
                {
                    if (originDistance < circleHeights[i])
                    {
                        hitInfo.hitTileY = i - 1;
                        break;
                    }
                }
    
                segmentSize = (circleHeights[hitInfo.hitTileY] * 2.0f * Mathf.PI) / width;
                tangentDistance = ((originMapAngle / (Mathf.PI * 2.0f)) * width);
    
                hitInfo.hitTileX = (int)tangentDistance;
                hitInfo.hitTileX = (hitInfo.hitTileX + 1) % width;
    
                len -= segmentSize * (Mathf.Ceil(tangentDistance) - tangentDistance);
    
                while (hitInfo.hitTileX < width && len >= 0)
                {
                    if (GetTile(hitInfo.hitTileX, hitInfo.hitTileY) != 0)
                    {
                        hitInfo.hitNormal = -GetTangentFromTileCoordinate(hitInfo.hitTileX, hitInfo.hitTileY);
    
                        hitInfo.hitPosition = position + 
                            circleNormals[hitInfo.hitTileX] * originDistance;
    
                        hitInfo.hitDistance = (origin - hitInfo.hitPosition).magnitude;
                        return true;
                    }
    
                    len -= segmentSize;
    
                    hitInfo.hitTileX++;
                }
            }
            else if (direction == TileDirection.Left)
            {
                target = origin + originTangent * len;
                targetdx = target.x - position.x;
                targetdy = target.y - position.y;
                targetDistance = Mathf.Sqrt(targetdx * targetdx + targetdy * targetdy);
    
                if (originDistance > circleHeights[circleHeights.Length - 1])
                {
                    //Origin point outside, not hit!
                    return false;
                }
    
                for (int i = 1; i < circleHeights.Length; i++)
                {
                    if (originDistance < circleHeights[i])
                    {
                        hitInfo.hitTileY = i - 1;
                        break;
                    }
                }
    
                segmentSize = (circleHeights[hitInfo.hitTileY] * 2.0f * Mathf.PI) / width;
                tangentDistance = ((originMapAngle / (Mathf.PI * 2.0f)) * width);
    
                hitInfo.hitTileX = (int)tangentDistance;
                hitInfo.hitTileX = (hitInfo.hitTileX - 1) % width;
                if (hitInfo.hitTileX < 0)
                    hitInfo.hitTileX += width;
    
                len -= segmentSize * (tangentDistance - Mathf.Floor(tangentDistance));
    
                while (hitInfo.hitTileX >= 0 && len >= 0)
                {
                    if (GetTile(hitInfo.hitTileX, hitInfo.hitTileY) != 0)
                    {
                        hitInfo.hitNormal = GetTangentFromTileCoordinate(hitInfo.hitTileX + 1, hitInfo.hitTileY);
    
                        hitInfo.hitPosition = position + 
                            circleNormals[(hitInfo.hitTileX + 1) % width] * originDistance;
    
                        hitInfo.hitDistance = (origin - hitInfo.hitPosition).magnitude;
                        return true;
                    }
    
                    len -= segmentSize;
    
                    hitInfo.hitTileX--;
                }
            }
            else if (direction == TileDirection.Up)
            {
                target = origin + originNormal * len;
                targetdx = target.x - position.x;
                targetdy = target.y - position.y;
                targetDistance = Mathf.Sqrt(targetdx * targetdx + targetdy * targetdy);
    
                if (originDistance > circleHeights[circleHeights.Length - 1])
                {
                    //Origin point outside, not hit!
                    return false;
                }
    
                hitInfo.hitTileX = (int) ((originMapAngle / (Mathf.PI * 2.0f)) * width);
                hitInfo.hitTileX = hitInfo.hitTileX % width;
    
                for (int i = 1; i < circleHeights.Length; i++)
                {
                    if (originDistance < circleHeights[i])
                    {
                        hitInfo.hitTileY = i;
                        len -= circleHeights[i] - originDistance;
                        break;
                    }
                }
    
                while (hitInfo.hitTileY < height && len >= 0)
                {
                    if (GetTile(hitInfo.hitTileX, hitInfo.hitTileY) != 0)
                    {
                        hitInfo.hitNormal = -originNormal;
                        hitInfo.hitPosition = position + originNormal * circleHeights[hitInfo.hitTileY];
                        hitInfo.hitDistance = (origin - hitInfo.hitPosition).magnitude;
                        return true;
                    }
    
                    if (hitInfo.hitTileY < height - 1 )
                        len -= (circleHeights[hitInfo.hitTileY + 1] - circleHeights[hitInfo.hitTileY]);
    
                    hitInfo.hitTileY++;
                }
            }
            else if (direction == TileDirection.Down)
            {
                target = origin - originNormal * len;
                targetdx = target.x - position.x;
                targetdy = target.y - position.y;
                targetDistance = Mathf.Sqrt(targetdx * targetdx + targetdy * targetdy);
    
                if (/*originDistance > circleHeights[circleHeights.Length - 1] &&*/
                    targetDistance > circleHeights[circleHeights.Length - 1])
                {
                    //Target outside, no hit!
                    return false;
                }
    
                hitInfo.hitTileX = (int) ((originMapAngle / (Mathf.PI * 2.0f)) * width);
                hitInfo.hitTileX = hitInfo.hitTileX % width;
    
                for (int i = circleHeights.Length - 1; i >= 1; i--)
                {
                    if (originDistance > circleHeights[i])
                    {
                        hitInfo.hitTileY = i - 1;
                        len -= originDistance - circleHeights[i];
                        break;
                    }
                }
    
                while (hitInfo.hitTileY >= 0 && len > 0)
                {
                    if (GetTile(hitInfo.hitTileX, hitInfo.hitTileY) != 0)
                    {
                        hitInfo.hitNormal = originNormal;
                        hitInfo.hitPosition = position + originNormal * circleHeights[hitInfo.hitTileY + 1];
                        hitInfo.hitDistance = (origin - hitInfo.hitPosition).magnitude;
                        return true;
                    }
    
                    if (hitInfo.hitTileY > 0)
                        len -= (circleHeights[hitInfo.hitTileY] - circleHeights[hitInfo.hitTileY - 1]);
                    hitInfo.hitTileY--;
                }
    
                if (hitInfo.hitTileY < 0 && len >= circleHeights[1] - circleHeights[0])
                {
                    //Core hit!
                    hitInfo.hitTileY = 0;
                    hitInfo.hitNormal = originNormal;
                    hitInfo.hitPosition = position + originNormal * circleHeights[hitInfo.hitTileY];
                    hitInfo.hitDistance = (origin - hitInfo.hitPosition).magnitude;
                    return true;
                }
            }
    
            return false;
        }
        
        public virtual void Recycle()
        {
            listener = null;
        }
    }
}
