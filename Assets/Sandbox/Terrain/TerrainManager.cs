using UnityEngine;
using Unity.Entities;

public class TerrainManager : MonoBehaviour
{
    TerrainComponent terrainComponent = new TerrainComponent { };
    EntityManager entityManager;

    private void Awake()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void Start()
    {
        AddTile();
    }

    public void AddTile()
    {
        Entity tile = entityManager.CreateEntity();
        entityManager.SetName(tile, new Unity.Collections.FixedString64Bytes("Tile"));
        entityManager.AddSharedComponent<TerrainComponent>(tile, terrainComponent);
    }

    public class TerrainManagerBaker : Baker<TerrainManager>
    {
        public override void Bake(TerrainManager authoring)
        {
            AddSharedComponent<TerrainComponent>(authoring.terrainComponent);
        }
    }
}
