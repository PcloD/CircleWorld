using System;
using UnityEngine;

namespace UniverseEngine
{
    public class DataPools
    {
        static public DataArrayPoolTemplate<Vector3> poolVector3 = new DataArrayPoolTemplate<Vector3>(1024);
        static public DataArrayPoolTemplate<Vector2> poolVector2 = new DataArrayPoolTemplate<Vector2>(1024);
        static public DataArrayPoolTemplate<Color32> poolColor32 = new DataArrayPoolTemplate<Color32>(1024);
        static public DataArrayPoolTemplate<float> poolFloat = new DataArrayPoolTemplate<float>(1024);
        static public DataArrayPoolTemplate<byte> poolByte = new DataArrayPoolTemplate<byte>(1024);
        static public DataArrayPoolTemplate<int> poolInt = new DataArrayPoolTemplate<int>(1024);
    }
}

