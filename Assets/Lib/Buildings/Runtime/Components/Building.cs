using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Earth.Buildings
{
    public struct BuildingComponent : IComponentData, IEnableableComponent
    {
        public float3 center;
    }
}
