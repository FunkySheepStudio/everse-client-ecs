using System;
using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Earth
{
    [Serializable]
    public struct GPSCoordinates : IComponentData
    {
        public double2 Value;
    }
}
