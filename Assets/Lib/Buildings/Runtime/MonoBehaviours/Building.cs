using UnityEngine;
using Unity.Entities;

namespace FunkySheep.Earth.Buildings
{
    public class Building : MonoBehaviour
    {
    }

    public class BakeBuilding : Baker<Building>
    {
        public override void Bake(Building authoring)
        {
        }
    }
}
