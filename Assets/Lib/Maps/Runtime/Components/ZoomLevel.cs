using System;
using Unity.Entities;

namespace FunkySheep.Maps
{
    [Serializable]
    public struct ZoomLevel : IComponentData
    {
        public int Value;
    }
}
