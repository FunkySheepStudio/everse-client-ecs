using Unity.Entities;

namespace FunkySheep.Earth.Buildings
{
    public struct BuildingComponent : IComponentData
    {
        public int id;
        public Entity prefab;
    }
}
