using Game.Player;
using Unity.Entities;

public partial class DisableGridItems : SystemBase
{
    EntityQuery entityQuery;

    protected override void OnCreate()
    {
        entityQuery = GetEntityQuery(typeof(CurrentGridPosition));
    }

    protected override void OnUpdate()
    {
        var currentPositions = entityQuery.ToComponentDataArray<CurrentGridPosition>(Unity.Collections.Allocator.TempJob);
        for (int i = 0; i < currentPositions.Length; i++)
        {
            Entities
            .WithSharedComponentFilter<GridPosition>(new GridPosition { Value = currentPositions[i].Value })
            .ForEach((Entity entity, in GridPosition gridPosition) =>
            {
                World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(entity);
            })
            .WithoutBurst()
            .WithStructuralChanges()
            .Run();
        }
    }
}
