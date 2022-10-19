using Unity.Entities;

namespace FunkySheep.Earth.Buildings
{
    public struct BuildingPrefab : IComponentData
    {
        public Entity cornerLeft;
        public Entity cornerRight;
    }
}
