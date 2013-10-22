using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniverseEngine
{
    public class Universe
    {
        public const int MAX_THINGS = 32767;

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
        
        private IUniverseListener listener;
        
        public ushort StartingPlanet
        {
            get { return startingPlanet; }
        }
        
        public Avatar Avatar
        {
            get { return avatar; }
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
            time += deltaTime;
            
            UpdatePositions(0, 0, 0, time);
            
            for (int i = 0; i < planets.Count; i++)
                planets[i].Update(deltaTime);
            
            for (int i = 0; i < tilemapObjects.Count; i++)
                tilemapObjects[i].Update(deltaTime);
        }

        private int UpdatePositions(int index, float x, float y, float time)
        {
            int childs = things[index].childs;

            float angle = things[index].angle * 0.000174532925f; //(degrees to radians / 100)
            float distance = things[index].distance;

            float normalizedOrbitalPeriod;
            
            if (things[index].orbitalPeriod != 0)
                normalizedOrbitalPeriod = time / things[index].orbitalPeriod;
            else
                normalizedOrbitalPeriod = 0;
            
            normalizedOrbitalPeriod = normalizedOrbitalPeriod - (int)normalizedOrbitalPeriod;

            float normalizedRotationPeriod;
            
            if (things[index].rotationPeriod != 0)
                normalizedRotationPeriod = time / things[index].rotationPeriod;
            else
                normalizedRotationPeriod = 0;
            
            normalizedRotationPeriod = normalizedRotationPeriod - (int)normalizedRotationPeriod;

            angle += 6.28318531f * normalizedOrbitalPeriod; //360 degrees to radians

            x += ((float) Math.Cos(angle)) * distance;
            y += ((float) Math.Sin(angle)) * distance;

            thingsPositions[index].x = x;
            thingsPositions[index].y = y;
            thingsPositions[index].rotation = normalizedRotationPeriod * 6.28318531f; //360 degrees to radian
            thingsPositions[index].radius = things[index].radius;

            //UnityEngine.Debug.Log(string.Format("Thing: {0} x: {1} y: {1}", index, x, y));

            index++;

            for (int i = 0; i < childs; i++)
                index = UpdatePositions(index, x, y, time);

            return index;
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
                planet.GetPositionFromTileCoordinate(0, planet.Height)
            );
            
            AddUniverseObject(avatar);
        }
        
        public void AddUniverseObject(UniverseObject universeObject)
        {
            tilemapObjects.Add(universeObject);
            
            if (listener != null)
                listener.OnUniverseObjectAdded(universeObject);
        }
    }
}


