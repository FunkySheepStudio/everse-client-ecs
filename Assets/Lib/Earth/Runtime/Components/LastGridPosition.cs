using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Earth
{
    public struct LastGridPosition : IComponentData
    {
        public int2 Value;
    }
}
