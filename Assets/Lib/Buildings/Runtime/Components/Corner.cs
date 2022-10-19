using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Earth.Buildings
{
    public struct CornerComponent : IBufferElementData, IEnableableComponent
    {
        public bool inside;
    }
}
