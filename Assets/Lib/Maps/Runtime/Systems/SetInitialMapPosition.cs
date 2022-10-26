using Unity.Collections;
using Unity.Entities;

namespace FunkySheep.Maps
{
    public partial class SetInitialMapPosition : SystemBase
    {
        EntityQuery query;
        protected override void OnCreate()
        {
            query = EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<MapPosition>(),
                ComponentType.Exclude<InitialMapPosition>()
                );
            RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in MapPosition mapPosition) =>
            {
                InitialMapPosition initialMapPosition;
                // Check if the initial map position have been set
                if (!TryGetSingleton<InitialMapPosition>(out initialMapPosition))
                {
                    initialMapPosition = new InitialMapPosition
                    {
                        Value = mapPosition.ToInt2()
                    };

                    Entity InitialMapSingleton = buffer.CreateEntity();
                    buffer.SetName(InitialMapSingleton, new FixedString32Bytes("InitialMap"));
                    buffer.AddComponent<InitialMapPosition>(InitialMapSingleton, initialMapPosition);
                }

                buffer.AddComponent<InitialMapPosition>(entity, initialMapPosition);
            })
            .WithNone<InitialMapPosition>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .WithoutBurst()
            .Run();
        }
    }
}
