using FunkySheep.Maps;
using FunkySheep.Transforms;
using Unity.Entities;

namespace FunkySheep.Earth
{
    public partial class InitMoveableEntities : SystemBase
    {
        protected override void OnCreate()
        {
            EntityQuery query = EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<LastLocalToWorldTransform>(),
                ComponentType.Exclude<LastGridPosition>()
                );
            RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in LastLocalToWorldTransform lastLocalToWorldTransform) =>
            {
                buffer.AddComponent<LastGridPosition>(entity);
                buffer.AddComponent<GridPositionChangedTag>(entity);
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .WithNone<LastGridPosition>()
            .ScheduleParallel();
        }
    }
}
