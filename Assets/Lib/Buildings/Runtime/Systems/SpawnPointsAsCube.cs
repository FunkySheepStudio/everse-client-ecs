using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;

namespace FunkySheep.Earth.Buildings
{
    public partial class SpawnPointsAsCube : SystemBase
    {
        protected override void OnUpdate()
        {
            BuildingPrefab building = GetSingleton<BuildingPrefab>();
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref DynamicBuffer<Point> points, in BuildingComponent buildingComponent) =>
            {
                // Set the farest point as first one
                points = SetFirstPoint(points, buildingComponent.center);

                // Set Points in clockwise order
                points = SetClockWise(points);


                for (int i = 0; i < points.Length; i++)
                {
                    // For debugging
                    if (i != points.Length - 1)
                    {
                        float colorIndice = (1f / points.Length) * i;

                        UnityEngine.Color color = new UnityEngine.Color(colorIndice, 0.5f, 0);
                        UnityEngine.Debug.DrawLine(points[i].Value, points[i + 1].Value, color, 10000);
                    } else
                    {
                        UnityEngine.Debug.DrawLine(points[0].Value, points[points.Length - 1].Value, UnityEngine.Color.black, 10000);
                    }

                    // Spawn Left Corner
                    Entity cornerLeft = buffer.Instantiate(building.cornerLeft);


                    float3 relativePos = points[(i + 1) % points.Length].Value - points[i].Value;
                    Quaternion LookAtRotation = Quaternion.LookRotation(relativePos);
                    Quaternion LookAtRotationOnly_Y = Quaternion.Euler(0, LookAtRotation.eulerAngles.y, 0);

                    buffer.SetComponent<LocalToWorldTransform>(cornerLeft, new LocalToWorldTransform
                    {
                        Value = new UniformScaleTransform
                        {
                            Rotation = LookAtRotationOnly_Y,
                            Position = points[i].Value,
                            Scale = 10
                        }
                    });

                    // Spawn right Corner
                    Entity cornerRight = buffer.Instantiate(building.cornerRight);


                    relativePos = points[i].Value - points[(i + 1) % points.Length].Value;
                    LookAtRotation = Quaternion.LookRotation(relativePos);
                    LookAtRotationOnly_Y = Quaternion.Euler(0, LookAtRotation.eulerAngles.y, 0);

                    buffer.SetComponent<LocalToWorldTransform>(cornerRight, new LocalToWorldTransform
                    {
                        Value = new UniformScaleTransform
                        {
                            Rotation = LookAtRotationOnly_Y,
                            Position = points[(i + 1) % points.Length].Value,
                            Scale = 10
                        }
                    });
                }
                buffer.DestroyEntity(entity);
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

    }
}
