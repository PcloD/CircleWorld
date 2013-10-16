using System;

namespace UniverseEngine
{
    public class UniverseGeneratorDefault : UniverseGenerator
    {
        protected override void AddGalaxy()
        {
            int galaxyOrbits = random.Next(10, 20);

            ushort galaxySafeRadius = things[currentThing].safeRadius;
            ushort solarSystemRadius = (ushort) (galaxySafeRadius / (galaxyOrbits * 2));

            for (int i = 0; i < galaxyOrbits; i++)
            {
                ushort solarSystemDistance = (ushort) ((galaxySafeRadius * i) / galaxyOrbits);

                int solarSystems = random.Next(
                    (Math.Max(i * 5, 1) + 1) / 2, 
                    Math.Max(i * 5, 1) + 1
                    );

                short solarSystemOrbitalPeriod = (short) random.Next(30, 60);
                if (random.Next(0, 2) == 0)
                    solarSystemOrbitalPeriod = (short) -solarSystemOrbitalPeriod;

                for (int j = 0; j < solarSystems; j++)
                {
                    ushort solarSystemAngle = (ushort) ((36000 * j) / solarSystems);

                    PushThing(ThingType.SolarSystem, solarSystemAngle, solarSystemDistance, 0, solarSystemOrbitalPeriod, 0, solarSystemRadius, 0);
                    {
                        int suns = random.Next(1, 3);

                        ushort solarSystemSafeRadius = things[currentThing].safeRadius;

                        ushort sunRadius = (ushort) random.Next(
                            (solarSystemRadius / 8) / 2,
                            (solarSystemRadius / 8)
                            );

                        if (suns == 1)
                        {
                            PushThing(ThingType.Sun, 0, 0, 0, 0, Planet.GetClosestValidRadius(sunRadius), 0, random.Next());
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

                                PushThing(ThingType.Sun, sunAngle, sunDistance, 0, sunOrbitalPerdiod, Planet.GetClosestValidRadius(sunRadius), 0, random.Next());
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

                            ushort planetRadius = (ushort) random.Next(planetSafeRadius / 16, planetSafeRadius / 9);

                            PushThing(ThingType.Planet, planetAngle, planetDistance, planetRotationPeriod, planetOrbitationPeriod, Planet.GetClosestValidRadius(planetRadius), planetSafeRadius, random.Next());
                            PopThing();
                        }

                    }
                    PopThing();
                }
            }
        }
    }
}