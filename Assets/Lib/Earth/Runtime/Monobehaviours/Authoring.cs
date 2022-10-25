using UnityEngine;
using Unity.Entities;
using FunkySheep.Earth.Terrain;
using FunkySheep.Network;
using Unity.Collections;
using Unity.Mathematics;

namespace FunkySheep.Earth
{
    public class Authoring : MonoBehaviour
    {
        public GPSCoordinates gpsCoordinates;
        public float tileSize;
        public int zoomLevel;
        public int2 mapPosition;

        public class EarthAuthoring : Baker<Authoring>
        {
            public override void Bake(Authoring authoring)
            {
                
            }
        }
    }
}
