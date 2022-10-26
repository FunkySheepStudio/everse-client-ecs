using Unity.Entities;
using FunkySheep.Maps;
using Unity.Transforms;
using Unity.Mathematics;

namespace FunkySheep.Earth
{
    public partial class UpdateGridPosition : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in GridPosition gridPosition, in LocalToWorldTransform localToWorldTransform, in TileSize tileSize) =>
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
                    buffer.SetSharedComponent<GridPosition>(entity, newGridPosition);
                }

            })
            .WithDeferredPlaybackSystem<EndInitializationEntityCommandBufferSystem>()
            .WithoutBurst()
            .Run();
        }
    }
}
