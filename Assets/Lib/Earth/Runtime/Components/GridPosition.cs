using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Earth
{
    public struct GridPosition : ISharedComponentData
    {
        public int2 Value;
    }
}
