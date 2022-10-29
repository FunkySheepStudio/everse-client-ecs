using FunkySheep.Earth;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using FunkySheep.Maps;
using Unity.Mathematics;

namespace FunkySheep.Terrain
{
    public partial class SetInitialLod : SystemBase
    {
        protected override void OnUpdate()
        {
            Entity player;
            if (!TryGetSingletonEntity<SpawnerTag>(out player))
                return;

            LocalToWorldTransform playerPosition;
            if (!GetComponentLookup<LocalToWorldTransform>().TryGetComponent(player, out playerPosition))
                return;

            TileSize tileSize;
            if (!TryGetSingleton<TileSize>(out tileSize))
                return;

            float stepDistance = tileSize.value / Manager.Instance.borderCount;
            float borderValue = math.sqrt(stepDistance * stepDistance);

            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in LocalToWorldTransform localToWorldTransform, in DebugTag debugTag) =>
            {
                Vector2 player2DPosition = new Vector2(
                    playerPosition.Value.Position.x,
                    playerPosition.Value.Position.z
                );

                Vector2 pointPosition = new Vector2(
                    localToWorldTransform.Value.Position.x,
                    localToWorldTransform.Value.Position.z
                );

                float distance = Vector2.Distance(pointPosition, player2DPosition);

                if (distance < 100)
                {
                    buffer.SetComponentEnabled<Lod0Tag>(entity, true);
                    buffer.SetComponentEnabled<Lod1Tag>(entity, false);
                    buffer.SetComponentEnabled<Lod2Tag>(entity, false);
                    if (distance > 100 - borderValue)
                        buffer.SetComponentEnabled<LodBorder>(entity, true);

                } else if (distance < 500)
                {
                    buffer.SetComponentEnabled<Lod1Tag>(entity, true);
                    buffer.SetComponentEnabled<Lod0Tag>(entity, false);
                    buffer.SetComponentEnabled<Lod2Tag>(entity, false);
                    if (distance > 500 - borderValue)
                        buffer.SetComponentEnabled<LodBorder>(entity, true);
                } else
                {
                    buffer.SetComponentEnabled<Lod2Tag>(entity, true);
                    buffer.SetComponentEnabled<Lod0Tag>(entity, false);
                    buffer.SetComponentEnabled<Lod1Tag>(entity, false);
                }
            })
                .WithNone<Lod0Tag>()
                .WithNone<Lod1Tag>()
                .WithNone<Lod2Tag>()
                .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
                .ScheduleParallel();

        }
    }
}
