using Unity.Entities;
using UnityEngine;

namespace FunkySheep.Earth.Buildings
{
    [AddComponentMenu("FunkySheep/Earth/Buildings/Manager")]
    public class Manager : MonoBehaviour
    {
        public FunkySheep.Types.String url;

        public void AddTile(Terrain.Tile terrainTile)
        {
            GameObject tileGo = new GameObject();
            tileGo.SetActive(false);
            tileGo.transform.parent = transform;
            tileGo.name = $"Tile {terrainTile.gridPosition.x} : {terrainTile.gridPosition.y}";
            Tile tileComponent = tileGo.AddComponent<Tile>();
            tileComponent.manager = this;
            tileComponent.gridPosition = terrainTile.gridPosition;
            tileGo.SetActive(true);
        }
    }
}
