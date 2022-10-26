using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Maps
{
    public struct MapPosition : IComponentData
    {
        public float2 Value;
        public int2 ToInt2()
        {
            return new int2 { x = (int)math.floor(Value.x), y = (int)math.floor(Value.y) };
        }
    }
}
