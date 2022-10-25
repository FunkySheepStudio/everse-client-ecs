using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Earth.Terrain
{
    public struct TilePosition : IComponentData
    {
        public int2 Value;
    }
}
