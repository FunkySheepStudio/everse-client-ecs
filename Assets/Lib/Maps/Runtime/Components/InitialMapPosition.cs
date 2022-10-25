using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Maps
{
    public struct InitialMapPosition : IComponentData
    {
        public int2 Value;
    }
}
