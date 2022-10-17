using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;

namespace FunkySheep.Earth.Terrain
{
    [AddComponentMenu("FunkySheep/Earth/Terrain/Manager")]
    public class Manager : MonoBehaviour
    {
        public Material material;
        public FunkySheep.Types.String heightsUrl;
        public FunkySheep.Types.String diffuseUrl;
        public AddedTileEvent addedTileEvent;

        UnityEngine.Terrain terrain;
        List<int2> tiles = new List<int2>();


        private void Start()
        {
            Earth.Manager.Instance.terrainManager = this;
        }

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
                tile.manager = this;
                tile.gridPosition = gridPosition;
                tileGo.SetActive(true);
            }
        }
    }
}
