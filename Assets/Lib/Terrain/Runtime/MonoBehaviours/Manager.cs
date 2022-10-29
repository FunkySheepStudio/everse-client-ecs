using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections;
using Unity.Jobs;
using FunkySheep.Types;
using FunkySheep.Maps;
using Unity.Entities;
using Unity.Transforms;

namespace FunkySheep.Terrain
{
    [AddComponentMenu("FunkySheep/Terrain/Manager")]
    public class Manager : Singleton<Manager>
    {
        public Material material;
        public FunkySheep.Types.String heightsUrl;
        public FunkySheep.Types.String diffuseUrl;
        public int borderCount;
        EntityManager entityManager;
        EntityArchetype archetype;

        public override void Awake()
        {
            base.Awake();
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            archetype = entityManager.CreateArchetype(
                typeof(LocalToWorldTransform),
                typeof(DebugTag),
                typeof(Lod0Tag),
                typeof(Lod1Tag),
                typeof(Lod2Tag),
                typeof(LodBorder)
            );
        }

        public void DownloadHeights(Entity entity, ZoomLevel zoomLevel, TileSize tileSize)
        {
            string[] variables = new string[3] { "zoom", "position.x", "position.y" };

            string[] values = new string[3] {
                zoomLevel.Value.ToString(),
                entityManager.GetComponentData<MapPosition>(entity).Value.x.ToString(),
                entityManager.GetComponentData<MapPosition>(entity).Value.y.ToString()
            };

            string url = heightsUrl.Interpolate(values, variables);

            StartCoroutine(FunkySheep.Network.Downloader.DownloadTexture(url, (fileID, texture) =>
            {
                ProcessHeights(entity, texture, tileSize);
            }));
        }

        public void ProcessHeights(Entity entity, Texture2D texture, TileSize tileSize)
        {
            NativeArray<Byte> bytes = texture.GetRawTextureData<Byte>();
            NativeArray<Entity> entities = entityManager.CreateEntity(archetype, bytes.Length, Allocator.Persistent);
            EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();

            borderCount = (int)math.sqrt(bytes.Length / 4);
            var setHeightsFromTextureJob = new SetHeightsFromTextureJob
            {
                count = (int)(bytes.Length / 4),
                borderCount = borderCount,
                tileSize = tileSize,
                uniformScale = entityManager.GetComponentData<LocalToWorldTransform>(entity).Value,
                ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter(),
                bytes = bytes,
                entities = entities
            };
            setHeightsFromTextureJob.Schedule(bytes.Length / 4, bytes.Length).Complete();
        }

        void DownloadDiffuse(MapPosition mapPosition, ZoomLevel zoomLevel)
        {
            string[] variables = new string[3] { "zoom", "position.x", "position.y" };

            string[] values = new string[3] {
                zoomLevel.Value.ToString(),
                mapPosition.Value.x.ToString(),
                mapPosition.Value.y.ToString()
            };

            string url = Manager.Instance.diffuseUrl.Interpolate(values, variables);

            StartCoroutine(FunkySheep.Network.Downloader.DownloadTexture(url, (fileID, texture) =>
            {
                ProcessDiffuse(texture);
            }));
        }

        public void ProcessDiffuse(Texture2D texture)
        {
        }

        [BurstCompile]
        struct SetHeightsFromTextureJob : IJobParallelFor
        {
            [NativeDisableContainerSafetyRestriction]
            [NativeDisableParallelForRestriction]
            public NativeArray<Byte> bytes;
            public UniformScaleTransform uniformScale;
            public TileSize tileSize;
            public int borderCount;
            public int count;
            public EntityCommandBuffer.ParallelWriter ecb;
            public NativeArray<Entity> entities;

            public void Execute(int i)
            {
                // Set position
                float3 position = new float3
                {
                    x = uniformScale.Position.x + (i % borderCount) * (tileSize.value / borderCount),
                    y = (math.floor(bytes[(i * 4) + 1] * 256.0f) + math.floor(bytes[(i * 4) + 2]) + (float)bytes[(i * 4) + 3] / 256f) - 32768.0f,
                    z = uniformScale.Position.z + (int)math.floor(i / borderCount) * (tileSize.value / borderCount)
                    
                };

                // Set neightbours
                ecb.SetComponent<LocalToWorldTransform>(i, entities[i], new LocalToWorldTransform
                {
                    Value = new UniformScaleTransform
                    {
                        Scale = uniformScale.Scale,
                        Position = position
                    }
                });

                if ((int)math.floor(i / borderCount) < borderCount - 1)
                {
                    ecb.AddComponent<TopEntity>(i, entities[i], new TopEntity
                    {
                        Value = entities[i + borderCount]
                    });
                }

                if ((int)math.floor(i / borderCount) != 0)
                {
                    ecb.AddComponent<BottomEntity>(i, entities[i], new BottomEntity
                    {
                        Value = entities[i - borderCount]
                    });
                }

                if ((i % borderCount) != borderCount - 1)
                {
                    ecb.AddComponent<RightEntity>(i, entities[i], new RightEntity
                    {
                        Value = entities[i + 1]
                    });
                }

                if ((i % borderCount) != 0)
                {
                    ecb.AddComponent<LeftEntity>(i, entities[i], new LeftEntity
                    {
                        Value = entities[i - 1]
                    });
                }

                ecb.SetComponentEnabled<Lod0Tag>(i, entities[i], false);
                ecb.SetComponentEnabled<Lod1Tag>(i, entities[i], false);
                ecb.SetComponentEnabled<Lod2Tag>(i, entities[i], false);
                ecb.SetComponentEnabled<LodBorder>(i, entities[i], false);
                ecb.SetComponentEnabled<DebugTag>(i, entities[i], true);
            }
        }
    }
}
