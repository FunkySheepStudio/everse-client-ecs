using Unity.Entities;
using FunkySheep.Maps;
using Unity.Transforms;
using Unity.Mathematics;

namespace FunkySheep.Earth
{
    public partial class SetInitialWorldPosition : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref LocalToWorldTransform localToWorldTransform, in MapPosition mapPosition, in InitialMapPosition initialMapPosition, in TileSize tileSize) =>
            {
                float2 mapOffset = mapPosition.Value - initialMapPosition.Value;

                localToWorldTransform.Value.Position = new Unity.Mathematics.float3
                {
                    x = mapOffset.x * tileSize.value,
                    y = localToWorldTransform.Value.Position.y,
                    z = tileSize.value - mapOffset.y * tileSize.value // Since map coordinates are reversed on Y axis
                };

                GridPosition gridPosition = new GridPosition
                {
                    Value = new int2
                    {
                        x = (int)math.floor((localToWorldTransform.Value.Position.x / tileSize.value)),
                        y = (int)math.floor((localToWorldTransform.Value.Position.z / tileSize.value))
                    }
                };

                buffer.AddSharedComponent<GridPosition>(entity, gridPosition);
            })
            .WithNone<GridPosition>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .WithoutBurst()
            .Run();
        }
    }
}
