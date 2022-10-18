using Unity.Entities;

namespace FunkySheep.Earth.Buildings
{
    public struct BuildingComponent : IComponentData
    {
        public Entity prefab;
    }
}
