using System;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using System.Drawing;
using System.Linq;

namespace FunkySheep.Earth.Terrain
{
    [AddComponentMenu("FunkySheep/Earth/Terrain/Tile")]
    [RequireComponent(typeof(UnityEngine.Terrain))]
    [RequireComponent(typeof(UnityEngine.TerrainCollider))]
    public class Tile : MonoBehaviour
    {
        public Manager manager;
        public int2 gridPosition;
        public bool heightUpdated = false;
        UnityEngine.Terrain terrain;

        private void Awake()
        {
            terrain = GetComponent<UnityEngine.Terrain>();
            terrain.enabled = false;
            terrain.allowAutoConnect = true;
            terrain.materialTemplate = Instantiate<Material>(manager.material);

            terrain.terrainData = new TerrainData();
            GetComponent<UnityEngine.TerrainCollider>().terrainData = terrain.terrainData;

            transform.position = new Vector3(
                gridPosition.x * Earth.Manager.Instance.tileSize,
                0,
                gridPosition.y * Earth.Manager.Instance.tileSize
            );

            DownloadHeights();
            DownloadDiffuse();
        }

        void DownloadHeights()
        {
            string[] variables = new string[3] { "zoom", "position.x", "position.y" };
            // Reverse the Y axis since the map Y axis is reversed from the unity Y axis
            string[] values = new string[3] {
                Earth.Manager.Instance.zoomLevel.ToString(),
                (Earth.Manager.Instance.mapPosition.x + gridPosition.x).ToString(),
                (Earth.Manager.Instance.mapPosition.y - gridPosition.y).ToString()
            };

            string url = manager.heightsUrl.Interpolate(values, variables);

            StartCoroutine(FunkySheep.Network.Downloader.DownloadTexture(url, (fileID, texture) =>
            {
                ProcessHeights(texture);
            }));
        }

        public void ProcessHeights(Texture2D texture)
        {

            NativeArray<Byte> bytes = texture.GetRawTextureData<Byte>();
            NativeArray<float> heights = new NativeArray<float>(bytes.Length / 4, Allocator.Temp);


            terrain.terrainData.heightmapResolution = (int)math.sqrt(heights.Length);
            terrain.terrainData.size = new Vector3(
               Earth.Manager.Instance.tileSize,
               8900,
               Earth.Manager.Instance.tileSize
           );

            var setHeightsFromTextureJob = new SetHeightsFromTextureJob
            {
                bytes = bytes,
                heights = heights
            };
            setHeightsFromTextureJob.Schedule(heights.Length, 64).Complete();

            float[,] height2D = ConvertArrayTo2DArray(heights.ToArray());
            heights.Dispose();

            terrain.terrainData.SetHeightsDelayLOD(0, 0, height2D);
            terrain.terrainData.SyncHeightmap();

            terrain.enabled = true;
            heightUpdated = true;
            gameObject.AddComponent<Connector>();
        }

        void DownloadDiffuse()
        {
            string[] variables = new string[3] { "zoom", "position.x", "position.y" };
            // Reverse the Y axis since the map Y axis is reversed from the unity Y axis
            string[] values = new string[3] {
                Earth.Manager.Instance.zoomLevel.ToString(),
                (Earth.Manager.Instance.mapPosition.x + gridPosition.x).ToString(),
                (Earth.Manager.Instance.mapPosition.y - gridPosition.y).ToString()
            };

            string url = manager.diffuseUrl.Interpolate(values, variables);

            StartCoroutine(FunkySheep.Network.Downloader.DownloadTexture(url, (fileID, texture) =>
            {
                ProcessDiffuse(texture);
            }));
        }

        public void ProcessDiffuse(Texture2D texture)
        {
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;
            terrain.materialTemplate.SetTexture("_MainTex", texture);
        }

        [BurstCompile]
        struct SetHeightsFromTextureJob : IJobParallelFor
        {
            [NativeDisableContainerSafetyRestriction]
            [NativeDisableParallelForRestriction]
            public NativeArray<Byte> bytes;
            [NativeDisableContainerSafetyRestriction]
            [NativeDisableParallelForRestriction]
            public NativeArray<float> heights;

            public void Execute(int i)
            {
                heights[i] = (math.floor(bytes[(i * 4) + 1] * 256.0f) + math.floor(bytes[(i * 4) + 2]) + bytes[(i * 4) + 3] / 256) - 32768.0f;
                heights[i] /= 8900;
            }
        }

        [BurstCompile]
        float[,] ConvertArrayTo2DArray(float[] flatArray)
        {
            int borderCount = (int)math.sqrt(flatArray.Length);
            float[,] array2D = new float[borderCount + 1, borderCount + 1];

            for (int i = 0; i < flatArray.Length; i++)
            {
                // Reverse X and Y since the downloaded image is readed from the top left
                int x = (int)math.floor(i / borderCount);
                int y = i % borderCount;

                array2D[x,y] = flatArray[i];

                // Add the next row since the terrain tile is 1 row bigger
                array2D[x + 1, y] = flatArray[i];
                array2D[x, y + 1] = flatArray[i];
                array2D[x + 1, y + 1] = flatArray[i];
            }

            return array2D;
        }

        /* public void ProcessHeights()
         {
             for (float x = 0; x < terrainResolution; x++)
             {
                 for (float y = 0; y < terrainResolution; y++)
                 {
                     float height = GetHeightFromColor(
                       Mathf.FloorToInt(x / terrainResolution * Mathf.Sqrt(pixels.Length)),
                       Mathf.FloorToInt(y / terrainResolution * Mathf.Sqrt(pixels.Length))
                     );
                     // Convert the resulting color value to an elevation in meters.
                     heights[
                         (int)x,
                         (int)y
                     ] = height;
                 }
             }
             heightsCalculated = true;
         }

         public float GetHeightFromColor(int x, int y)
         {
             Color32 color = pixels[
                 y +
                 x * (int)Mathf.Sqrt(pixels.Length)];

             float height = (Mathf.Floor(color.r * 256.0f) + Mathf.Floor(color.g) + color.b / 256) - 32768.0f;
             height /= 8900;

             return height;
         }*/
    }
}
