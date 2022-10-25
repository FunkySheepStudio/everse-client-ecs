using System;
using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Earth
{
    [Serializable]
    public struct GPSCoordinatesArray : IBufferElementData, IEnableableComponent
    {
        public GPSCoordinates Value;
    }
}
