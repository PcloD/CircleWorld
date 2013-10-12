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

            AddGalaxy();

            UpdateBrothers(0);
        }

        private void AddGalaxy()
        {
            int galaxyOrbits = 10;

            ushort galaxySafeRadius = things[currentThing].safeRadius;
            ushort solarSystemRadius = (ushort) (galaxySafeRadius / (galaxyOrbits * 2));

            for (int i = 0; i < galaxyOrbits; i++)
            {
                ushort solarSystemDistance = (ushort) ((galaxySafeRadius * i) / galaxyOrbits);

                int solarSystems = random.Next(
                    (Math.Max(i * 5, 1) + 1) / 2, 
                    Math.Max(i * 5, 1) + 1
                );

                short solarSystemOrbitalPerdiod = (short) random.Next(30, 60);
                if (random.Next(0, 2) == 0)
                    solarSystemOrbitalPerdiod = (short) -solarSystemOrbitalPerdiod;

                for (int j = 0; j < solarSystems; j++)
                {
                    ushort solarSystemAngle = (ushort) ((36000 * j) / solarSystems);

                    PushThing(ThingType.SolarSystem, solarSystemAngle, solarSystemDistance, 0, solarSystemOrbitalPerdiod, 0, solarSystemRadius, random.Next());
                    {
                        int suns = random.Next(1, 3);

                        ushort solarSystemSafeRadius = things[currentThing].safeRadius;

                        ushort sunRadius = (ushort) random.Next(
                            (solarSystemRadius / 8) / 2,
                            (solarSystemRadius / 8)
                        );

                        if (suns == 1)
                        {
                            PushThing(ThingType.Sun, 0, 0, 0, 1, sunRadius, 0, 0);
                            PopThing();
                        }
                        else
                        {
                            ushort sunDistance = (ushort) (sunRadius * 4 / 3);
                            short sunOrbitalPerdiod = (short) random.Next(30, 60);
                            if (random.Next(0, 2) == 0)
                                sunOrbitalPerdiod = (short) -sunOrbitalPerdiod;

                            for (int k = 0; k < suns; k++)
                            {
                                ushort sunAngle = (ushort) ((36000 * k) / suns);

                                PushThing(ThingType.Sun, sunAngle, sunDistance, 0, sunOrbitalPerdiod, sunRadius, 0, 0);
                                PopThing();
                            }
                        }

                        int planetsOrbits = random.Next(1, 8);

                        ushort planetSafeRadius = (ushort) ((solarSystemSafeRadius - sunRadius * 6) / (planetsOrbits * 2));

                        for (int l = 0; l < planetsOrbits; l++)
                        {
                            ushort planetDistance = (ushort) (sunRadius * 6 + ((solarSystemSafeRadius - sunRadius * 6) * l) / planetsOrbits);

                            ushort planetAngle = (ushort) random.Next(0, 36000);

                            short planetRotationPeriod = (short) random.Next(30, 60);
                            if (random.Next(0, 2) == 0)
                                planetRotationPeriod = (short) -planetRotationPeriod;

                            short planetOrbitationPeriod = (short) random.Next(30, 60);
                            if (random.Next(0, 2) == 0)
                                planetOrbitationPeriod = (short) -planetOrbitationPeriod;

                            int planetSeed = random.Next();

                            ushort planetRadius = (ushort) random.Next(planetSafeRadius / 16, planetSafeRadius / 9);

                            PushThing(ThingType.Planet, planetAngle, planetDistance, planetRotationPeriod, planetOrbitationPeriod, planetRadius, planetSafeRadius, planetSeed);
                            PopThing();
                        }

                    }
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

        private void PushThing(ThingType type, ushort angle, ushort distance, short rotationPeriod, short orbitalPeriod, ushort radius, ushort safeRadius, int seed)
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

            x += ((float) Math.Cos(angle)) * distance;
            y += ((float) Math.Sin(angle)) * distance;

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


