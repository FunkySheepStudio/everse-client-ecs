using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace FunkySheep.Earth.Buildings
{
    public partial class CalculatePointsCoordinates : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in BuildingTag buildingTag, in DynamicBuffer<GPSCoordinatesArray> gPSCoordinatesArray) =>
            {
                DynamicBuffer<Point> points = buffer.AddBuffer<Point>(entity);

                for (int i = 0; i < gPSCoordinatesArray.Length; i++)
                {
                    float3 point = Earth.Manager.GetWorldPosition(gPSCoordinatesArray[i].Value);
                    float? height = Terrain.Manager.GetHeight(point);
                    if (height != null)
                    {
                        point.y = height.Value;
                        points.Add(
                            new Point
                            {
                                Value = point
                            }
                        );
                    }
                }

                buffer.RemoveComponent<GPSCoordinatesArray>(entity);
            })
            .WithoutBurst()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .Run();

        }
    }
}
