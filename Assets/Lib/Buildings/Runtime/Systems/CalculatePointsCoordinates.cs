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
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref BuildingComponent buildingComponent, in DynamicBuffer<GPSCoordinatesArray> gPSCoordinatesArray) =>
            {
                DynamicBuffer<Point> points = buffer.AddBuffer<Point>(entity);

                float3 center = new float3();

                for (int i = 0; i < gPSCoordinatesArray.Length - 1; i++)
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

                        center += point;
                    }
                }

                center /= points.Length;

                buffer.RemoveComponent<GPSCoordinatesArray>(entity);
                buffer.SetComponent<BuildingComponent>(entity, new BuildingComponent
                {
                    center = center
                });
                buffer.SetComponentEnabled<BuildingComponent>(entity, true);
            })
            .WithoutBurst()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .Run();

        }
    }
}
