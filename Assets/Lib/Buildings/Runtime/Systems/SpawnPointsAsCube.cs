using Unity.Entities;

namespace FunkySheep.Earth.Buildings
{
    public partial class SpawnPointsAsCube : SystemBase
    {
        EntityManager entityManager;

        protected override void OnCreate()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref DynamicBuffer<Point> points, in BuildingComponent building) =>
            {
                entityManager.RemoveComponent<BuildingComponent>(entity);
            })
            .WithStructuralChanges()
            .Run();
        }
    }
}
