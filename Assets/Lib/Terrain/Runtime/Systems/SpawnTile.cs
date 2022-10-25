using Unity.Entities;
using FunkySheep.Earth;
using Unity.Collections;

namespace FunkySheep.Terrain
{
    public partial class SpawnTile : SystemBase
    {
        Entity terrainEntity;
        EntityManager entityManager;

        protected override void OnCreate()
        {
            entityManager = World.EntityManager;
            terrainEntity = entityManager.CreateEntity();
            entityManager.SetName(terrainEntity, new FixedString64Bytes("Terrain"));
            entityManager.AddComponent<Terrain>(terrainEntity);
            entityManager.AddBuffer<Tile>(terrainEntity);

        }
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in GridPosition gridPosition, in SpawnerTag spawnerTag) =>
            {
                DynamicBuffer<Tile> tiles = entityManager.GetBuffer<Tile>(terrainEntity, false);
                for (int i = 0; i < tiles.Length; i++)
                {
                    if (tiles[i].position.Equals(gridPosition))
                        return;
                }

                Tile terrainTile = new Tile
                {
                    position = gridPosition
                };

                tiles.Add(terrainTile);

                Entity tileEntity = buffer.CreateEntity();
                buffer.SetName(tileEntity, new FixedString64Bytes("Terrain - Tile - " + gridPosition.Value.ToString()));
                buffer.AddSharedComponent<GridPosition>(tileEntity, gridPosition);
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .WithoutBurst()
            .Run();
        }
    }
}
