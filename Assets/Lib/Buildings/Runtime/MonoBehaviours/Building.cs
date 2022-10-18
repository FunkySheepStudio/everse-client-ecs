using UnityEngine;
using Unity.Entities;

namespace FunkySheep.Earth.Buildings
{
    public class Building : MonoBehaviour
    {
        public GameObject buildingPrefab;
    }

    public class BakeBuilding : Baker<Building>
    {
        public override void Bake(Building authoring)
        {
            AddComponent<BuildingComponent>(
                new BuildingComponent
                {
                    prefab = GetEntity(authoring.buildingPrefab)
                }
            );
        }
    }
}
