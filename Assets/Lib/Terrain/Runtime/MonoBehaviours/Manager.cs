using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections;
using Unity.Jobs;
using FunkySheep.Types;
using FunkySheep.Maps;
using FunkySheep.Earth;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.Rendering;
using System.Security.Cryptography;

namespace FunkySheep.Terrain
{
    [AddComponentMenu("FunkySheep/Terrain/Manager")]
    public class Manager : Singleton<Manager>
    {
        public Material material;
        public FunkySheep.Types.String heightsUrl;
        public FunkySheep.Types.String diffuseUrl;
        EntityManager entityManager;

        public override void Awake()
        {
            base.Awake();
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
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

            EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();

            var setHeightsFromTextureJob = new SetHeightsFromTextureJob
            {
                count = (int)(bytes.Length / 4),
                borderCount = (int)math.sqrt(bytes.Length / 4),
                tileSize = tileSize,
                uniformScale = entityManager.GetComponentData<LocalToWorldTransform>(entity).Value,
                ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter(),
                bytes = bytes
            };
            setHeightsFromTextureJob.Schedule(bytes.Length / 4, 64).Complete();
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

            public void Execute(int i)
            {
                Entity entity = ecb.CreateEntity(i);
                ecb.AddComponent<LocalToWorldTransform>(i, entity, new LocalToWorldTransform
                {
                    Value = new UniformScaleTransform
                    {
                        Scale = uniformScale.Scale,
                        Position = new float3
                        {
                            x = uniformScale.Position.x + (int)math.floor(i / borderCount) * (tileSize.value / borderCount),
                            y = (math.floor(bytes[(i * 4) + 1] * 256.0f) + math.floor(bytes[(i * 4) + 2]) + bytes[(i * 4) + 3] / 256) - 32768.0f,
                            z = uniformScale.Position.z + (i % borderCount) * (tileSize.value / borderCount)
                        }
                    }
                });
            }
        }
    }
}
