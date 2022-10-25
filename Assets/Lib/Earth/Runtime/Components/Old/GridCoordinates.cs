using System;
using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Earth
{
    [Serializable]
    public struct GridCoordinates : IComponentData
    {
        public int2 Value;
    }
}
