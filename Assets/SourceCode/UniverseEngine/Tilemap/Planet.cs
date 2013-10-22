using System;
using UnityEngine;

namespace UniverseEngine
{
    public class Planet : TilemapCircle
    {
        private float gravity = 10.0f;
        
        private Universe universe;
        private ushort thingIndex;
        
        public ushort ThingIndex
        {
            get { return thingIndex; }
        }
        
        public float Gravity
        {
            get { return gravity; }
            set { this.gravity = value; }
        }
        
        protected override void UpdateTiles ()
        {
            //System.Random random = new System.Random(Seed);
            
            PlanetType planetType = PlanetTypes.GetPlanetType((byte) (Math.Abs(Seed % 4)));
            
            byte tileId = planetType.mainTileId;
            
            for (int i = 0; i < tiles.Length; i++)
            {
                //if (random.NextDouble() > 0.95f)
                //    tiles[i] = 0;
                //else
                    tiles[i] = tileId;
            }        
        }
        
        public void InitPlanet(Universe universe, ushort thingIndex)
        {
            this.universe = universe;
            this.thingIndex = thingIndex;
            
            Thing thing = universe.GetThing(thingIndex);
            
            Init(thing.seed, GetPlanetHeightWithRadius(thing.radius));
            
            UpdatePlanetPosition();
        }
        
        public override void Recycle ()
        {
            base.Recycle ();
            
            universe = null;
            thingIndex = 0;
        }
        
        public void Update(float deltaTime)
        {
            UpdatePlanetPosition();
            
            if (listener != null)
                listener.OnTilemapParentChanged(deltaTime);
        }
        
        private void UpdatePlanetPosition()
        {
            ThingPosition thing = universe.GetThingPosition(thingIndex);
            
            position.x = thing.x;
            position.y = thing.y;
            rotation = thing.rotation;
        }
        
        static private ushort[] validHeights = new ushort[] { 8, 16, 32, 64, 128 };
        static private ushort[] validRadius;
        
        static public ushort GetClosestValidRadius(ushort radius)
        {
            InitValidRadius();
            
            for (int i = 0; i < validRadius.Length; i++)
            {
                if (validRadius[i] > radius)
                {
                    if (i == 0)
                        return validRadius[0];
                    else
                        return validRadius[i - 1];
                }
            }
            
            return 0;
        }   
        

        static private void InitValidRadius()
        {
            if (validRadius == null)
            {
                validRadius = new ushort[validHeights.Length];
                for (int i = 0; i < validHeights.Length; i++)
                    validRadius[i] = GetRadiusFromPlanetHeight(validHeights[i]);
                
                for (int i = 0; i < validHeights.Length; i++)
                    if (GetPlanetHeightWithRadius(validRadius[i]) != validHeights[i])
                        Debug.LogError(string.Format("Invalid validRadius[] initialization, expected {0}, got {1}", validHeights[i], GetPlanetHeightWithRadius(validRadius[i])));
            }
        }
                
        static public ushort GetPlanetHeightWithRadius(ushort radius)
        {
            InitValidRadius();
                
            for (int i = 0; i < validRadius.Length; i++)
                if (radius == validRadius[i])
                    return validHeights[i];
            
            return 0;
        }
        
        static private ushort GetRadiusFromPlanetHeight(ushort height)
        {
            int width = (((int)((float)height * Mathf.PI * 2.0f)) / 4) * 4;
            
            float height0 = (height - 1) * TILE_SIZE;
            float k = -((width / (Mathf.PI * 2.0f))) / (1 - (width / (Mathf.PI * 2.0f)));
            
            float r = height0 * Mathf.Pow(k, (float) height);
            
            return (ushort) r;
        } 
    }
}


