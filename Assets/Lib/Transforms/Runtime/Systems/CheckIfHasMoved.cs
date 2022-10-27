using FunkySheep.Earth;
using FunkySheep.Maps;
using Unity.Entities;
using Unity.Transforms;

namespace FunkySheep.Transforms
{
    public partial class CheckIfHasMoved : SystemBase
    {
        EntityQuery query;
        protected override void OnCreate()
        {
            query = EntityManager.CreateEntityQuery(
                ComponentType.ReadWrite<LastLocalToWorldTransform>(),
                ComponentType.ReadOnly<LocalToWorldTransform>()
                );
            RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref LastLocalToWorldTransform lastLocalToWorldTransform, in LocalToWorldTransform LocalToWorldTransform) =>
            {
                if (!lastLocalToWorldTransform.Value.Value.Position.Equals(LocalToWorldTransform.Value.Position))
                {
                    lastLocalToWorldTransform.Value.Value.Position = LocalToWorldTransform.Value.Position;
                    buffer.SetComponentEnabled<HasMovedTag>(entity, true);
                } else
                {
                    buffer.SetComponentEnabled<HasMovedTag>(entity, false);
                }
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
