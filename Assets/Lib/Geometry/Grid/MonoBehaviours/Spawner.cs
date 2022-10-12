using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

namespace FunkySheep.Geometry.Grid
{
    public class Spawner : MonoBehaviour
    {
        public int size;
        public GameObject prefab;
    }

    public class SpawnerBaker : Baker<Spawner>
    {
        public override void Bake(Spawner authoring)
        {
            Entity prefab = GetEntity(authoring.prefab);

            AddComponent<Grid>(new Grid
            {
                size = authoring.size,
                prefab = prefab
            });

            AddSharedComponent<GridPosition>(new GridPosition
            {
                Value = new int2
                {
                    x = (int)authoring.transform.position.x,
                    y = (int)authoring.transform.position.z
                }
            });
        }
    }
}
