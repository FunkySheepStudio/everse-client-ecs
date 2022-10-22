using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace FunkySheep.Earth
{
    [AddComponentMenu("FunkySheep/Earth/manager")]
    public class Manager : FunkySheep.Types.Singleton<Manager>
    {
        public bool InitAtStartup = false;
        public GPSCoordinates gpsCoordinates;
        public float tileSize;
        public int zoomLevel = 15;
        public int2 mapPosition;

        public override void Awake()
        {
            base.Awake();
            if (InitAtStartup)
                Init(gpsCoordinates);
        }

        public void Init(GPSCoordinates gpsCoordinates)
        {
            Instance.tileSize = (float)Utils.TileSize(zoomLevel, gpsCoordinates.Value.x);
            Instance.mapPosition = Utils.GpsToMapInt2(zoomLevel, gpsCoordinates.Value.x, gpsCoordinates.Value.y);
        }

        /// <summary>
        /// Return the world position given GPS Coordinates
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static float3 GetWorldPosition(GPSCoordinates gpsCoordinates)
        {
            float2 calculatedMapPosition = Utils.GpsToMapRealFloat2(Manager.Instance.zoomLevel, gpsCoordinates.Value);
            float2 mapOffset = calculatedMapPosition - Manager.Instance.mapPosition;

            return new float3(
                mapOffset.x * Manager.Instance.tileSize,
                0,
                 Manager.Instance.tileSize - mapOffset.y * Manager.Instance.tileSize // Since map coordinates are reversed on Y axis
            );
        }


        /// <summary>
        /// Get the position on the grid relative to the transform
        /// </summary>
        /// <param name="translation"></param>
        /// <returns></returns>
        public static int2 GetTilePosition(float3 position)
        {
            return new int2
            {
                x = (int)math.floor((position.x / Manager.Instance.tileSize)),
                y = (int)math.floor((position.z / Manager.Instance.tileSize))
            };
        }

        /// <summary>
        /// Get the map position given a grid position
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        public static int2 GetMapPosition(int2 gridPosition)
        {
            // Reverse the Y axis since the map Y axis is reversed from the unity Y axis
            return new int2
            {
                x = Manager.Instance.mapPosition.x + gridPosition.x,
                y = Manager.Instance.mapPosition.y - gridPosition.y
            };
        }
    }
}
