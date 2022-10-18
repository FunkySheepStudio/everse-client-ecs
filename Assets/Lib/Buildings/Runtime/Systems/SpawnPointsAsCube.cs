using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

namespace FunkySheep.Earth.Buildings
{
    public partial class SpawnPointsAsCube : SystemBase
    {
        protected override void OnUpdate()
        {
            BuildingPrefab building = GetSingleton<BuildingPrefab>();
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref DynamicBuffer<Point> points) =>
            {
                for (int i = 0; i < points.Length; i++)
                {
                    Entity point = buffer.Instantiate(building.prefab);
                    buffer.SetComponent<LocalToWorldTransform>(point, new LocalToWorldTransform
                    {
                        Value = new UniformScaleTransform
                        {
                            Position = points[i].Value,
                            Scale = 1
                        }
                    });
                }
                buffer.DestroyEntity(entity);
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();

        }
    }
}
