using Unity.Entities;
using FunkySheep.Earth;
using FunkySheep.Maps;
using Unity.Collections;
using Unity.Transforms;

namespace FunkySheep.Terrain
{
    [UpdateAfter(typeof(UpdateMapPosition))]
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

            EntityQuery query = EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<GridPosition>(),
                ComponentType.ReadOnly<MapPosition>(),
                ComponentType.ReadOnly<SpawnerTag>(),
                ComponentType.ReadOnly<GridPositionChangedTag>()
                );
            RequireForUpdate(query);

        }
        protected override void OnUpdate()
        {
            TileSize tileSize;
            if (!TryGetSingleton<TileSize>(out tileSize))
                return;

            ZoomLevel zoomLevel;
            if (!TryGetSingleton<ZoomLevel>(out zoomLevel))
                return;

            Entities.ForEach((Entity entity, in GridPosition gridPosition, in MapPosition mapPosition, in SpawnerTag spawnerTag, in GridPositionChangedTag gridPositionChangedTag) =>
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

                Entity tileEntity = entityManager.CreateEntity();
                entityManager.SetName(tileEntity, new FixedString64Bytes("Terrain - Tile - " + gridPosition.Value.ToString()));
                entityManager.AddSharedComponent<GridPosition>(tileEntity, gridPosition);
                entityManager.AddComponent<MapPosition>(tileEntity);
                entityManager.SetComponentData<MapPosition>(tileEntity, mapPosition);
                entityManager.AddComponent<LocalToWorldTransform>(tileEntity);
                entityManager.SetComponentData<LocalToWorldTransform>(tileEntity, new LocalToWorldTransform
                {
                    Value = new UniformScaleTransform
                    {
                        Scale = 1,
                        Position = new Unity.Mathematics.float3
                        {
                            x = gridPosition.Value.x * tileSize.value,
                            y = 0,
                            z = gridPosition.Value.y * tileSize.value,
                        }
                    }
                });

                Manager.Instance.DownloadHeights(tileEntity, zoomLevel, tileSize);
            })
            .WithStructuralChanges()
            .WithoutBurst()
            .Run();
        }
    }
}
