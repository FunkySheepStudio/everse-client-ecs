using FunkySheep.Earth;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;


namespace FunkySheep.Terrain
{
    public partial class SetInitialLod : SystemBase
    {
        protected override void OnUpdate()
        {
            Entity player = GetSingletonEntity<SpawnerTag>();
            LocalToWorldTransform playerPosition;
            GetComponentLookup<LocalToWorldTransform>().TryGetComponent(player, out playerPosition);

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
                } else if (distance < 500)
                {
                    buffer.SetComponentEnabled<Lod1Tag>(entity, true);
                    buffer.SetComponentEnabled<Lod0Tag>(entity, false);
                    buffer.SetComponentEnabled<Lod2Tag>(entity, false);
                } else
                {
                    buffer.SetComponentEnabled<Lod2Tag>(entity, true);
                    buffer.SetComponentEnabled<Lod0Tag>(entity, false);
                    buffer.SetComponentEnabled<Lod1Tag>(entity, false);
                }
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();

        }
    }
}
