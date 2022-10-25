using FunkySheep.Earth;
using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Maps
{
    public partial class CalculateMapPosition : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in GPSCoordinates gPSCoordinates, in ZoomLevel zoomLevel) =>
            {
                double latitude = gPSCoordinates.Value.y;
                MapPosition mapPosition = new MapPosition { };
                mapPosition.Value.x = (float)((latitude + 180.0) / 360.0 * (1 << zoomLevel.Value));
                mapPosition.Value.y = (float)((1.0 - math.log(math.tan(latitude * math.PI / 180.0) + 1.0 / math.cos(latitude * math.PI / 180.0)) / math.PI) / 2.0 * (1 << zoomLevel.Value));

                buffer.AddComponent<MapPosition>(entity, mapPosition);
            })
            .WithNone<MapPosition>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
