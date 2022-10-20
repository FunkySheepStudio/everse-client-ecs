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
                // Discard the change if the building is on a terrain that do not exist
                for (int i = 0; i < gPSCoordinatesArray.Length - 1; i++)
                {
                    float3 point = Earth.Manager.GetWorldPosition(gPSCoordinatesArray[i].Value);
                    float? height = Terrain.Manager.GetHeight(point);
                    if (height == null)
                    {
                        return;
                    }
                }

                DynamicBuffer<Point> points = buffer.AddBuffer<Point>(entity);

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

                        if (i == 0)
                        {
                            buildingComponent.minHeight = point.y;
                            buildingComponent.maxHeight = point.y;
                        } else if (point.y < buildingComponent.minHeight)
                        {
                            buildingComponent.minHeight = point.y;
                        } else if (point.y > buildingComponent.maxHeight)
                        {
                            buildingComponent.maxHeight = point.y;
                        }

                        buildingComponent.center += point;
                    }
                }

                buildingComponent.center /= points.Length;

                buffer.RemoveComponent<GPSCoordinatesArray>(entity);
                buffer.SetComponentEnabled<BuildingComponent>(entity, true);
                buffer.SetComponentEnabled<WallsTag>(entity, true);
            })
            .WithoutBurst()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .Run();

        }
    }
}
