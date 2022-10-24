using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;

namespace FunkySheep.Earth.Buildings
{
    [RequireMatchingQueriesForUpdate]
    public partial class CreateBuildingGeometry : SystemBase
    {
        protected override void OnUpdate()
        {
            BuildingPrefab building = GetSingleton<BuildingPrefab>();
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref DynamicBuffer<Point> points, in BuildingComponent buildingComponent, in WallsTag wallsTag) =>
            {
                float area = Area(points);
                //Discard Colinear points;
                for (int i = 0; i < points.Length; i++)
                {
                    float2 a = points[ClampListIndex(i - 1, points.Length)].GetPos2D_XZ();
                    float2 b = points[i].GetPos2D_XZ();
                    float2 c = points[ClampListIndex(i + 1, points.Length)].GetPos2D_XZ();

                    if (Geometry.utils.IsCollinear(a, b, c))
                        points.RemoveAt(i);
                }


                // Set the farest point as first one
                points = SetFirstPoint(points, buildingComponent.center);

                // Set Points in clockwise order
                points = SetClockWise(points);

                float heightOffset = area;

                for (int i = 0; i < points.Length; i++)
                {
                    float3 relativePos;
                    Quaternion LookAtRotation = Quaternion.identity;
                    Quaternion LookAtRotationOnly_Y;
                    float4x4 transform;

                    // Spawn the wall
                    Entity wall = buffer.Instantiate(building.wall);

                    float3 position;
                    if (points[i].Value.y < points[(i + 1) % points.Length].Value.y)
                    {
                        position.y = points[i].Value.y;
                    } else
                    {
                        position.y = points[(i + 1) % points.Length].Value.y;
                    }

                    position.x = (points[i].Value.x + points[(i + 1) % points.Length].Value.x) / 2;
                    position.z = (points[i].Value.z + points[(i + 1) % points.Length].Value.z) / 2;

                    float wallWidth = math.distance(
                        points[i].Value * new float3(1, 0, 1),
                        points[(i + 1) % points.Length].Value * new float3(1, 0, 1)
                    );

                    relativePos = points[i].Value - points[(i + 1) % points.Length].Value;
                    if (!relativePos.Equals(float3.zero))
                        LookAtRotation = Quaternion.LookRotation(relativePos);
                    LookAtRotationOnly_Y = Quaternion.Euler(0, LookAtRotation.eulerAngles.y, 0);
                    buffer.RemoveComponent<LocalToWorldTransform>(wall);
                    transform = float4x4.TRS(
                        position,
                        LookAtRotationOnly_Y,
                        new float3(1, buildingComponent.maxHeight - position.y + heightOffset, wallWidth)
                    );
                    buffer.SetComponent<LocalToWorld>(wall, new LocalToWorld
                    {
                        Value = transform
                    });

                    // Update the point to create the roof
                    points[i] = new Point
                    {
                        Value = new float3
                        {
                            x = points[i].Value.x,
                            y = buildingComponent.maxHeight + heightOffset,
                            z = points[i].Value.z
                        }
                    };

                }

                buffer.SetComponentEnabled<WallsTag>(entity, false);
                buffer.SetComponentEnabled<RoofTag>(entity, true);
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();

        }

        //Clamp list indices
        //Will even work if index is larger/smaller than listSize, so can loop multiple times
        public static int ClampListIndex(int index, int listSize)
        {
            index = ((index % listSize) + listSize) % listSize;

            return index;
        }

        /// <summary>
        /// Set the first point of the building (the farest from the center)
        /// </summary>
        public static DynamicBuffer<Point> SetFirstPoint(DynamicBuffer<Point> points, float3 center)
        {
            int maxPointIndex = 0;
            for (int i = 0; i < points.Length; i++)
            {
                if (math.distance(center, points[maxPointIndex].Value) < math.distance(center, points[i].Value))
                {
                    maxPointIndex = i;
                }
            }

            NativeArray<Point> tempPoints = new NativeArray<Point>(points.Length, Allocator.Temp);
            tempPoints.CopyFrom(points.AsNativeArray());
            points.Clear();

            for (int i = 0; i < tempPoints.Length; i++)
            {
                points.Add(tempPoints[(i + maxPointIndex) % tempPoints.Length]);
            }

            return points;
        }

        /// <summary>
        /// If the Vector Array is clockwise, return it
        /// </summary>
        /// <returns></returns>
        public static DynamicBuffer<Point> SetClockWise(DynamicBuffer<Point> points)
        {
            bool result = Geometry.utils.IsTriangleOrientedClockwise(points[points.Length - 1].GetPos2D_XZ(), points[0].GetPos2D_XZ(), points[1].GetPos2D_XZ());

            if (result)
            {
                NativeArray<Point> tempPoints = new NativeArray<Point>(points.Length, Allocator.Temp);
                tempPoints.CopyFrom(points.AsNativeArray());
                points.Clear();

                for (int i = tempPoints.Length - 1; i >=0 ; i--)
                {
                    points.Add(tempPoints[i]);
                }
            }

            return points;
        }

        /// <summary>
        /// Calculate the building area
        /// </summary>
        /// <returns></returns>
        public static float Area(DynamicBuffer<Point> points)
        {
            float area = 0;

            for (int i = 0; i < points.Length; i++)
            {
                area += Vector2.Distance(points[i].GetPos2D_XZ(), points[(i + 1) % points.Length].GetPos2D_XZ());
            }

            return area;
        }
    }
}
