using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FunkySheep.Earth.Terrain
{
    [AddComponentMenu("FunkySheep/Earth/Terrain/Manager")]
    public class Manager : MonoBehaviour
    {
        public Material material;
        public FunkySheep.Types.String heightsUrl;
        public FunkySheep.Types.String diffuseUrl;

        UnityEngine.Terrain terrain;

        private void Awake()
        {
            Earth.Manager.Instance.terrainManager = this;
        }

        public void AddTile(int2 gridPosition)
        {
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
