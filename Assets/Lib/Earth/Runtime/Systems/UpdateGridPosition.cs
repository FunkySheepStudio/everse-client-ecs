using Unity.Entities;
using FunkySheep.Maps;
using Unity.Transforms;
using Unity.Mathematics;
using FunkySheep.Transforms;

namespace FunkySheep.Earth
{
    public partial class UpdateGridPosition : SystemBase
    {
        protected override void OnCreate()
        {
            EntityQuery query = EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<GridPosition>(),
                ComponentType.ReadOnly<LocalToWorldTransform>(),
                ComponentType.ReadOnly<HasMovedTag>()
                );
            RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            TileSize tileSize;
            if (!TryGetSingleton<TileSize>(out tileSize))
                return;

            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref LastGridPosition lastGridPosition, in GridPosition gridPosition, in LocalToWorldTransform localToWorldTransform, in HasMovedTag hasMoved) =>
            {
                GridPosition newGridPosition = new GridPosition
                {
                    Value = new int2
                    {
                        x = (int)math.floor((localToWorldTransform.Value.Position.x / tileSize.value)),
                        y = (int)math.floor((localToWorldTransform.Value.Position.z / tileSize.value))
                    }
                };

                if (!newGridPosition.Equals(gridPosition))
                {
                    lastGridPosition.Value = gridPosition.Value;
                    buffer.SetSharedComponent<GridPosition>(entity, newGridPosition);
                    buffer.SetComponentEnabled<GridPositionChangedTag>(entity, true);
                } else
                {
                    buffer.SetComponentEnabled<GridPositionChangedTag>(entity, false);
                }

            })
            .WithDeferredPlaybackSystem<EndInitializationEntityCommandBufferSystem>()
            .WithoutBurst()
            .Run();
        }
    }
}
