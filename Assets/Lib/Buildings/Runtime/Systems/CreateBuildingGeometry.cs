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
                // Set the farest point as first one
                points = SetFirstPoint(points, buildingComponent.center);

                // Set Points in clockwise order
                points = SetClockWise(points);

                float heightOffset = area / 4;

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
            if (points.Length <= 2)
            {
                return points;
            }

            int result = IsClockWise(points[1].Value, points[points.Length - 1].Value, points[0].Value);
            if (result < 0)
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
        /// Check if two Vector2 are clockwise around the origin
        /// </summary>
        /// <param name="first">First Vector2</param>
        /// <param name="second">Second Vector2</param>
        /// <param name="origin">The Vector2 orgin to compare from</param>
        /// <returns>Return 1 if clockwise, -1 if anticlockwise, 0 if aligned</returns>
        public static int IsClockWise(float3 first, float3 second, float3 origin)
        {
            float3 firstOffset = origin - first;
            float3 secondOffset = origin - second;

            float angleOffset = UnityEngine.Vector3.SignedAngle(firstOffset, secondOffset, UnityEngine.Vector3.up);

            if (angleOffset > 0)
            {
                return 1;
            }
            else if (angleOffset < 0)
            {
                return -1;
            }
            else
            {
                return 0;
            }

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
