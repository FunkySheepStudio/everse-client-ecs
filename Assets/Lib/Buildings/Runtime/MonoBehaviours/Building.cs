using UnityEngine;
using Unity.Entities;

namespace FunkySheep.Earth.Buildings
{
    public class Building : MonoBehaviour
    {
        public GameObject wall;
    }

    public class BakeBuilding : Baker<Building>
    {
        public override void Bake(Building authoring)
        {
            AddComponent<BuildingPrefab>(
                new BuildingPrefab
                {
                    wall = GetEntity(authoring.wall)
                }
            );
        }
    }
}
