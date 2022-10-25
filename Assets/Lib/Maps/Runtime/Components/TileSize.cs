using System;
using Unity.Entities;

namespace FunkySheep.Maps
{
    [Serializable]
    public struct TileSize : IComponentData
    {
        public float value;
    }
}
