using System;

namespace Universe
{
    public struct Thing
    {
        public ushort type; //type of thing (use ThingType!)
        public ushort childs; //number of childs

        public ushort parent; //Index of parent
        public ushort nextBrother; //Index of next Thing with same parent

        public ushort angle; //Rotation relative to parent (centidegrees (100 -> 1 degree, 36000 -> 360 degrees)
        public ushort distance; //Distance relative to parent

        public ushort rotationPeriod; //seconds to complete a full rotation around itself
        public ushort orbitalPeriod; //seconds to complete a full rotation around the parent

        public ushort radius; //Radius
        public ushort safeRadius; //Radius at which things can orbit this thing without colliding with any brother

        public int seed; //Seed used to create the thing
    }
}
