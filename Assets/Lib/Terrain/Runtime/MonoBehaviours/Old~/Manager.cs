using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;
using Unity.Burst;

namespace FunkySheep.Earth.Terrain
{
    [AddComponentMenu("FunkySheep/Earth/Terrain/Manager")]
    public class Manager : FunkySheep.Types.Singleton<Manager>
    {
        public Material material;
        public FunkySheep.Types.String heightsUrl;
        public FunkySheep.Types.String diffuseUrl;
        public AddedTileEvent addedTileEvent;

        UnityEngine.Terrain terrain;
        List<int2> tiles = new List<int2>();

        public void AddTile(int2 gridPosition)
        {
            if (!tiles.Contains(gridPosition))
            {
                tiles.Add(gridPosition);
                GameObject tileGo = new GameObject();
                tileGo.SetActive(false);
                tileGo.name = $"Tile {gridPosition.x} : {gridPosition.y}";
                tileGo.transform.parent = transform;
                Tile tile = tileGo.AddComponent<Tile>();
                tile.gridPosition = gridPosition;
                tileGo.SetActive(true);
            }
        }

        [BurstCompile]
        public static float? GetHeight(float3 position)
        {
            foreach (UnityEngine.Terrain terrain in UnityEngine.Terrain.activeTerrains)
            {
                UnityEngine.Bounds bounds = terrain.terrainData.bounds;
                Vector2 terrainMin = new Vector2(
                  bounds.min.x + terrain.transform.position.x,
                  bounds.min.z + terrain.transform.position.z
                );

                Vector2 terrainMax = new Vector2(
                  bounds.max.x + terrain.transform.position.x,
                  bounds.max.z + terrain.transform.position.z
                );

                if (position.x >= terrainMin.x && position.z >= terrainMin.y && position.x <= terrainMax.x && position.z <= terrainMax.y)
                {
                    if (terrain.GetComponent<Tile>().heightUpdated == true)
                    {
                        return terrain.terrainData.GetInterpolatedHeight(
                          (position.x - terrainMin.x) / (terrainMax.x - terrainMin.x),
                          (position.z - terrainMin.y) / (terrainMax.y - terrainMin.y)
                        );
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }
    }
}
