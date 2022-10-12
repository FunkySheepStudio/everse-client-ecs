using Unity.Entities;
using Unity.Collections;
using System;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Transforms;
using UnityEngine.UIElements;
using Unity.Mathematics;

namespace FunkySheep.Geometry.Grid
{
    public partial class Generate : SystemBase
    {
        EntityManager entityManager;
        ComponentLookup<LocalToWorldTransform> componentLookup;

        protected override void OnCreate()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            componentLookup = GetComponentLookup<LocalToWorldTransform>();
        }

        protected override void OnUpdate()
        {
            var world = World.Unmanaged;
            Entities.ForEach((Entity entity, in Grid grid, in GridPosition gridPosition) =>
            {
                int tilesCount = grid.size * grid.size;
                var tiles = CollectionHelper.CreateNativeArray<Entity, RewindableAllocator>(tilesCount, ref world.UpdateAllocator);
                entityManager.Instantiate(grid.prefab, tiles);
                entityManager.AddSharedComponent<GridPosition>(tiles, gridPosition);

                componentLookup.Update(this);
                SetTilesPositionJob setTilesPositionJob = new SetTilesPositionJob
                {
                    componentLookup = componentLookup,
                    entities = tiles,
                    size = grid.size,
                    position = gridPosition.Value
                };

                Dependency = setTilesPositionJob.Schedule(tilesCount, 64, Dependency);

                entityManager.DestroyEntity(entity);
            })
                .WithStructuralChanges()
                .Run();
        }

        [BurstCompile]
        struct SetTilesPositionJob : IJobParallelFor
        {
            [NativeDisableContainerSafetyRestriction]
            [NativeDisableParallelForRestriction]
            public ComponentLookup<LocalToWorldTransform> componentLookup;
            public NativeArray<Entity> entities;
            public int size;
            public int2 position;

            public void Execute(int index)
            {
                var entity = entities[index];
                int x = index % size * 2;
                x += position.x;
                int z = index / size * 2;
                z += position.y;

                var pos = new float3(x, 0, z);

                var newPosition = new LocalToWorldTransform
                {
                    Value = new UniformScaleTransform
                    {
                        Position = pos,
                        Scale = 1
                    }
                };

                componentLookup[entity] = newPosition;

            }
        }
    }
}
