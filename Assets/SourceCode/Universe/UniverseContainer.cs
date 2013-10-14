using System;

namespace Universe
{
    public class UniverseContainer
    {
        public const int MAX_THINGS = 32767;

        public Thing[] things = new Thing[MAX_THINGS];
        public ThingPosition[] thingsPositions = new ThingPosition[MAX_THINGS];
        public ushort thingsAmount;

        public ushort[] thingsToRender = new ushort[MAX_THINGS];
        public ushort thingsToRenderAmount;
        
        public ushort startingPlanet;

        public void Create(int seed)
        {
            thingsAmount = new UniverseGeneratorDefault().Generate(seed, things);

            UpdateThingsToRender();
            
            startingPlanet = thingsToRender[200];
            
            UpdatePositions(0);
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

        public void UpdatePositions(float time)
        {
            UpdatePositions(0, 0, 0, time);
        }

        private int UpdatePositions(int index, float x, float y, float time)
        {
            int childs = things[index].childs;

            float angle = things[index].angle * 0.000174532925f; //(degrees to radians / 100)
            float distance = things[index].distance;

            float normalizedOrbitalPeriod = time / things[index].orbitalPeriod;
            normalizedOrbitalPeriod = normalizedOrbitalPeriod - (int)normalizedOrbitalPeriod;

            float normalizedRotationPeriod = time / things[index].rotationPeriod;
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
    }
}


