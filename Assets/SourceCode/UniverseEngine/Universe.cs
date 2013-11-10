//#define USE_SIN_TABLE

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniverseEngine
{
    public class Universe
    {
        public const int MAX_THINGS = 8192;

        private const float HALF_PI = Mathf.PI * 0.5f; //90 degress in radians
        private const float TWO_PI = Mathf.PI * 2.0f; //360 degress in radians
        private const float INV_TWO_PI = 1.0f / (Mathf.PI * 2.0f); 
        private const float DEG_TO_RAD_OVER_100 = 0.000174532925f; //(degrees to radians / 100)
        
        private const float POSITIONS_TIME_SCALE = 0.01f;

        private Thing[] things = new Thing[MAX_THINGS];
        private ThingPosition[] thingsPositions = new ThingPosition[MAX_THINGS];
        private ushort thingsAmount;

        private ushort[] thingsToRender = new ushort[MAX_THINGS];
        private ushort thingsToRenderAmount;
        
        private ushort startingPlanet;
        
        private List<Planet> planets = new List<Planet>();
        
        private List<UniverseObject> tilemapObjects = new List<UniverseObject>();
        
        private UniverseFactory universeFactory = new UniverseFactory();
        
        private float time;
        
        private Avatar avatar;
        
        private Ship ship;
        
        private IUniverseListener listener;
        
        public ushort StartingPlanet
        {
            get { return startingPlanet; }
        }
        
        public Avatar Avatar
        {
            get { return avatar; }
        }
        
        public Ship Ship
        {
            get { return ship; }
        }
        
        public Thing[] Things
        {
            get { return things; }
        }

        public ThingPosition[] ThingsPositions
        {
            get { return thingsPositions; }
        }
        
        public ushort[] ThingsToRender
        {
            get { return thingsToRender; }
        }
        
        public ushort ThingsToRenderAmount
        {
            get { return thingsToRenderAmount; }
        }
        
        public IUniverseListener Listener
        {
            get { return listener; }
            set { listener = value; }
        }
        
        public void Init(int seed, IUniverseListener listener)
        {
            this.listener = listener;
            
            time = 0.0f;
            
            thingsAmount = new UniverseGeneratorDefault().Generate(seed, things);

            UpdateThingsToRender();
            
            startingPlanet = thingsToRender[1];
            
            UpdateUniverse(0);
            
            AddAvatar();
            
            AddShip();
        }

        private void UpdateThingsToRender()
        {
            thingsToRenderAmount = 0;
            for (int i = 0; i < thingsAmount; i++)
            {
                ThingType type = (ThingType)things[i].type;

                if (type == ThingType.Sun || type == ThingType.Planet || type == ThingType.Moon)
                    thingsToRender[thingsToRenderAmount++] = (ushort) i;
            }
        }

        public void UpdateUniverse(float deltaTime)
        {
#if USE_SIN_TABLE
            if (sinTable == null)
                InitSinTable();
#endif

            time += deltaTime;

            UEProfiler.BeginSample("Universe.UpdatePositions");
            UpdatePositions(time);
            UEProfiler.EndSample();

            for (int i = 0; i < planets.Count; i++)
                planets[i].Update(deltaTime);
            
            for (int i = 0; i < tilemapObjects.Count; i++)
                tilemapObjects[i].Update(deltaTime);
        }

#if USE_SIN_TABLE
        private const int SIN_TABLE_LEN = 512;
        private const float SIN_TABLE_LEN_F = SIN_TABLE_LEN;
        private const float SIN_TABLE_INDEX = INV_TWO_PI * SIN_TABLE_LEN_F;

        static private float[] sinTable;
        static private float[] cosTable;

        static private void InitSinTable()
        {
            sinTable = new float[SIN_TABLE_LEN];
            cosTable = new float[SIN_TABLE_LEN];

            for (int i = 0; i < sinTable.Length; i++)
            {
                sinTable[i] = Mathf.Sin((i * TWO_PI) / SIN_TABLE_LEN_F);
                cosTable[i] = Mathf.Cos((i * TWO_PI) / SIN_TABLE_LEN_F);

                //Debug.Log(sinTable[i]);
            }
        }

        static private float Sin(float angle)
        {
            return sinTable[(int)(angle * SIN_TABLE_INDEX) % SIN_TABLE_LEN];
        }

        static private float Cos(float angle)
        {
            //return cosTable[(int)(angle * SIN_TABLE_INDEX) % SIN_TABLE_LEN];

            return sinTable[(int)((angle + HALF_PI) * SIN_TABLE_INDEX) % SIN_TABLE_LEN];
        }
#endif

        private void UpdatePositions(float time)
        {
            time *= POSITIONS_TIME_SCALE;
            
            for (int index = 1; index < thingsAmount; index++)
            {
                Thing thing = things[index];

                float parentX = thingsPositions[thing.parent].x;
                float parentY = thingsPositions[thing.parent].y;

                float angle = thing.angle * DEG_TO_RAD_OVER_100;
                float distance = thing.distance;

                float normalizedOrbitalPeriod = time * thing.orbitalPeriodInv;
                normalizedOrbitalPeriod -= (int)normalizedOrbitalPeriod;

                float normalizedRotationPeriod = time * thing.rotationPeriodInv;
                normalizedRotationPeriod -= (int)normalizedRotationPeriod;

                angle += TWO_PI * normalizedOrbitalPeriod; //360 degrees to radians

#if USE_SIN_TABLE
                if (angle < 0)
                    angle += TWO_PI;

                thingsPositions[index].x = parentX + Cos(angle) * distance;
                thingsPositions[index].y = parentY + Sin(angle) * distance;
#else
                thingsPositions[index].x = parentX + ((float)Math.Cos(angle)) * distance;
                thingsPositions[index].y = parentY + ((float)Math.Sin(angle)) * distance;
#endif
                thingsPositions[index].rotation = normalizedRotationPeriod * TWO_PI; //360 degrees to radian
                thingsPositions[index].radius = thing.radius;
            }
        }

        public Thing GetThing(ushort thingIndex)
        {
            return things[thingIndex];
        }
        
        public ThingPosition GetThingPosition(ushort thingIndex)
        {
            return thingsPositions[thingIndex];
        }
        
        public Planet GetPlanet(ushort thingIndex)
        {
            for (int i = 0; i < planets.Count; i++)
                if (planets[i].ThingIndex == thingIndex)
                    return planets[i];
            
            if (things[thingIndex].type != (ushort) ThingType.Sun &&
                things[thingIndex].type != (ushort) ThingType.Planet &&
                things[thingIndex].type != (ushort) ThingType.Moon)
            {
                return null;
            }
            
            Planet planet = universeFactory.GetPlanet(Planet.GetPlanetHeightWithRadius(things[thingIndex].radius));
            
            planet.InitPlanet(this, thingIndex);
            
            planets.Add(planet);
            
            return planet;
        }
        
        public void ReturnPlanet(Planet planet)
        {
            if (planets.Remove(planet))
            {
                if (listener != null)
                    listener.OnPlanetReturned(planet);
                
                universeFactory.ReturnPlanet(planet);
            }
        }
        
        private void AddAvatar()
        {
            Planet planet = GetPlanet(startingPlanet);
            
            avatar = universeFactory.GetAvatar();
            avatar.Init(
                new Vector2(0.75f, 1.05f),
                planet,
                FollowParentParameters.Default,
                planet.GetPositionFromTileCoordinate(0, planet.Height),
                0.0f
            );
            
            AddUniverseObject(avatar);
        }

        private void AddShip()
        {
            ship = universeFactory.GetShip();
            ship.Init(
                new Vector2(1.0f, 1.0f),
                avatar.parent,
                FollowParentParameters.None,
                avatar.parent.GetPositionFromTileCoordinate(0, avatar.parent.Height + 5),
                Mathf.PI * 0.5f
            );
            
            AddUniverseObject(ship);
        }   
        
        public void AddUniverseObject(UniverseObject universeObject)
        {
            tilemapObjects.Add(universeObject);
            
            if (listener != null)
                listener.OnUniverseObjectAdded(universeObject);
        }
        
        public int FindClosestRenderedThing(Vector2 worldPos, float searchRadius)
        {
            ushort closestThingIndex = ushort.MaxValue;
            float closestThingDistance = float.MaxValue;
            
            for (ushort i = 0; i < thingsToRenderAmount; i++)
            {
                ThingPosition thingPosition = thingsPositions[thingsToRender[i]];
                
                float distance = (worldPos - new Vector2(thingPosition.x, thingPosition.y)).sqrMagnitude;
                
                if (distance < (thingPosition.radius + searchRadius) * (thingPosition.radius + searchRadius) && 
                    distance < closestThingDistance)
                {
                    closestThingIndex = thingsToRender[i];
                    closestThingDistance = distance;
                }
            }
            
            if (closestThingIndex != ushort.MaxValue)
                return closestThingIndex;
            else
                return -1;
        }
        
        public List<ushort> FindClosestRenderedThings(Vector2 worldPos, float searchRadius, List<ushort> toReturn)
        {
            if (toReturn == null)
                toReturn = new List<ushort>();
            else
                toReturn.Clear();
            
            for (ushort i = 0; i < thingsToRenderAmount; i++)
            {
                ThingPosition thingPosition = thingsPositions[thingsToRender[i]];
                
                float distance = (worldPos - new Vector2(thingPosition.x, thingPosition.y)).sqrMagnitude;
                
                if (distance < (thingPosition.radius + searchRadius) * (thingPosition.radius + searchRadius))
                    toReturn.Add(thingsToRender[i]);
            }
            
            return toReturn;
        }        
    }
}


