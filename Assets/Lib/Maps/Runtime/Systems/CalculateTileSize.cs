using FunkySheep.Earth;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

namespace FunkySheep.Maps
{
    public partial class CalculateTileSize : SystemBase
    {
        EntityQuery query;
        protected override void OnCreate()
        {
            query = EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<GPSCoordinates>()
            );
            RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            ZoomLevel zoomLevel;
            if (!TryGetSingleton<ZoomLevel>(out zoomLevel))
                return;

            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in GPSCoordinates gPSCoordinates) =>
            {
                TileSize tileSize = new TileSize { };
                if (!TryGetSingleton<TileSize>(out tileSize))
                {
                    tileSize.value = (float)(156543.03 / math.pow(2, zoomLevel.Value) * math.cos(math.PI * 2 / 360 * gPSCoordinates.Value.y) * 256);
                    Entity tileSizeEntity = buffer.CreateEntity();
                    buffer.SetName(tileSizeEntity, new FixedString32Bytes("TileSize"));
                    buffer.AddComponent<TileSize>(tileSizeEntity, tileSize);
                    Enabled = false;
                }
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .WithoutBurst()
            .Run();
        }
    }

}
