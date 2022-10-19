using UnityEngine;
using Unity.Entities;

namespace FunkySheep.Earth.Buildings
{
    public class Building : MonoBehaviour
    {
        public GameObject cornerLeft;
        public GameObject cornerRight;
        public GameObject wall;
    }

    public class BakeBuilding : Baker<Building>
    {
        public override void Bake(Building authoring)
        {
            AddComponent<BuildingPrefab>(
                new BuildingPrefab
                {
                    cornerLeft = GetEntity(authoring.cornerLeft),
                    cornerRight = GetEntity(authoring.cornerRight),
                    wall = GetEntity(authoring.wall)
                }
            );
        }
    }
}
