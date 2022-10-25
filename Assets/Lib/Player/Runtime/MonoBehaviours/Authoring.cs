using FunkySheep.Earth;
using FunkySheep.Maps;
using FunkySheep.Terrain;
using Unity.Entities;
using UnityEngine;

namespace FunkySheep.Player
{
    public class Authoring : MonoBehaviour
    {
        public GPSCoordinates gPSCoordinates;
        public ZoomLevel zoomLevel;

        public class PlayerAuthoring : Baker<Authoring>
        {
            public override void Bake(Authoring authoring)
            {
                AddComponent<GPSCoordinates>(authoring.gPSCoordinates);
                AddComponent<ZoomLevel>(authoring.zoomLevel);
                AddComponent<SpawnerTag>(new SpawnerTag { });
            }
        }
    }
}
