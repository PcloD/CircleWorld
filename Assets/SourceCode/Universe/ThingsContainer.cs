using System;

namespace Universe
{
    public class ThingsContainer
    {
        public const int MAX_THINGS = 8192;

        public Thing[] things = new Thing[MAX_THINGS];
        public ThingPosition[] thingsPositions = new ThingPosition[MAX_THINGS];

        public ushort thingsAmount;

        private Random random;
        private ushort currentThing;

        public void Create(int seed)
        {
            things[0].type = (ushort)ThingType.Galaxy;
            things[0].orbitalPeriod = 1;
            things[0].safeRadius = ushort.MaxValue;
            currentThing = 0;
            thingsAmount = 1;

            random = new Random(seed);

            AddSolarSystems();

            UpdateBrothers(0);
        }

        private void AddSolarSystems()
        {
            int orbits = 10;

            ushort safeRadius = things[currentThing].safeRadius;

            ushort solarSystemSize = (ushort) (safeRadius / (orbits * 2));

            for (int i = 0; i < orbits; i++)
            {
                ushort solarSystemDistance = (ushort) ((safeRadius * i) / orbits);

                int solarSystems = 5;

                ushort solarSystemOrbitalPerdiod = (ushort) random.Next(10, 60);

                for (int j = 0; j < solarSystems; j++)
                {
                    ushort angle = (ushort) ((36000 * j) / solarSystems);

                    PushThing(ThingType.SolarSystem, angle, solarSystemDistance, 0, solarSystemOrbitalPerdiod, solarSystemSize, solarSystemSize, random.Next());

                    PopThing();
                }
            }
        }

        private ushort UpdateBrothers(ushort index)
        {
            int childs = things[index].childs;
            index++;

            ushort prevBrother = 0;

            for (int i = 0; i < childs; i++)
            {
                prevBrother = index;

                index = UpdateBrothers(index);

                if (i + 1 < childs)
                    things[prevBrother].nextBrother = index;
            }

            return index;
        }

        private void PushThing(ThingType type, ushort angle, ushort distance, ushort rotationPeriod, ushort orbitalPeriod, ushort radius, ushort safeRadius, int seed)
        {
            things[currentThing].childs++;

            things[thingsAmount].parent = currentThing;
            things[thingsAmount].type = (ushort)type;

            things[thingsAmount].angle = angle;
            things[thingsAmount].distance = distance;
            things[thingsAmount].rotationPeriod = rotationPeriod;
            things[thingsAmount].orbitalPeriod = orbitalPeriod;
            things[thingsAmount].radius = radius;
            things[thingsAmount].safeRadius = safeRadius;
            things[thingsAmount].seed = seed;

            currentThing = thingsAmount;
            thingsAmount++;
        }

        private void PopThing()
        {
            currentThing = things[currentThing].parent;
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

            x += ((float) Math.Sin(angle)) * distance;
            y += ((float) Math.Cos(angle)) * distance;

            thingsPositions[index].x = x;
            thingsPositions[index].y = y;
            thingsPositions[index].rotation = normalizedRotationPeriod * 6.28318531f; //360 degrees to radian

            //UnityEngine.Debug.Log(string.Format("Thing: {0} x: {1} y: {1}", index, x, y));

            index++;

            for (int i = 0; i < childs; i++)
                index = UpdatePositions(index, x, y, time);

            return index;
        }
    }
}


