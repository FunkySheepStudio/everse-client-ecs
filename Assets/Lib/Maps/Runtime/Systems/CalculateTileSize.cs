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
                ComponentType.ReadOnly<GPSCoordinates>(),
                ComponentType.ReadOnly<ZoomLevel>(),
                ComponentType.Exclude<TileSize>()
                );
            RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in GPSCoordinates gPSCoordinates, in ZoomLevel zoomLevel) =>
            {
                TileSize tileSize = new TileSize { };
                if (!TryGetSingleton<TileSize>(out tileSize))
                {
                    tileSize.value = (float)(156543.03 / math.pow(2, zoomLevel.Value) * math.cos(math.PI * 2 / 360 * gPSCoordinates.Value.y) * 256);
                    Entity tileSizeEntity = buffer.CreateEntity();
                    buffer.SetName(tileSizeEntity, new FixedString32Bytes("TileSize"));
                    buffer.AddComponent<TileSize>(tileSizeEntity, tileSize);
                }

                buffer.AddComponent<TileSize>(entity, tileSize);
                buffer.RemoveComponent<ZoomLevel>(entity);
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .WithoutBurst()
            .Run();
        }
    }

}
