using System;
using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Earth
{
    [Serializable]
    public struct InitialGpsCoordinates : IComponentData
    {
        public double2 Value;
    }
}
