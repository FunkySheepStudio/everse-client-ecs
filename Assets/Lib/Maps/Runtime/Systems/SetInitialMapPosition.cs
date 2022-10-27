using Unity.Collections;
using Unity.Entities;

namespace FunkySheep.Maps
{
    public partial class SetInitialMapPosition : SystemBase
    {
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
                    Enabled = false;
                }
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .WithoutBurst()
            .Run();
        }
    }
}
