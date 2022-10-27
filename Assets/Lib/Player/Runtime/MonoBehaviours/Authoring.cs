using FunkySheep.Earth;
using FunkySheep.Transforms;
using Unity.Entities;
using UnityEngine;

namespace FunkySheep.Player
{
    public class Authoring : MonoBehaviour
    {
        public GPSCoordinates gPSCoordinates;

        public class PlayerAuthoring : Baker<Authoring>
        {
            public override void Bake(Authoring authoring)
            {
                AddComponent<GPSCoordinates>(authoring.gPSCoordinates);
                AddComponent<LastLocalToWorldTransform>();
                AddComponent<HasMovedTag>();
                AddComponent<SpawnerTag>(new SpawnerTag { });
            }
        }
    }
}
