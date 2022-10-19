using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;
using Unity.Entities.UniversalDelegates;

namespace FunkySheep.Earth.Buildings
{
    public class Tile : MonoBehaviour
    {
        public Manager manager;
        public int2 gridPosition;

        EntityManager entityManager;

        private void Awake()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void Start()
        {
            Download();
        }

        void Download()
        {
            string url = InterpolatedUrl();
            StartCoroutine(FunkySheep.Network.Downloader.Download(url, (fileID, file) =>
            {
                string fileStr = System.Text.Encoding.Default.GetString(file);
                JsonOsmRoot buildings = JsonUtility.FromJson<JsonOsmRoot>(fileStr);

                for (int i = 0; i < buildings.elements.Length; i++)
                {
                    AddBuildingGeometryEntity(buildings.elements[i]);
                }
            }));
        }

        public void AddBuildingGeometryEntity(JsonOsmElement element)
        {
            if (element.geometry != null)
            {
                Entity buildingEntity = entityManager.CreateEntity();
                entityManager.AddBuffer<GPSCoordinatesArray>(buildingEntity);
                DynamicBuffer<GPSCoordinatesArray> points = entityManager.GetBuffer<GPSCoordinatesArray>(buildingEntity);

                points.CopyFrom(new NativeArray<GPSCoordinatesArray>(element.AsGPSCoordinatesArray(), Allocator.Temp));

                entityManager.AddComponent<BuildingComponent>(buildingEntity);
            }

            // Used for relations
            if (element.members != null)
            {
                for (int i = 0; i < element.members.Length; i++)
                {
                    AddBuildingGeometryEntity(element.members[i]);
                }
            }
        }

        /// <summary>
        /// Interpolate the url inserting the boundaries and the types of OSM data to download
        /// </summary>
        /// <param boundaries="boundaries">The gps boundaries to download in</param>
        /// <returns>The interpolated Url</returns>
        public string InterpolatedUrl()
        {
            int2 mapPosition = FunkySheep.Earth.Manager.GetMapPosition(gridPosition);

            double[] gpsBoundaries = FunkySheep.Earth.Utils.CaclulateGpsBoundaries(
                FunkySheep.Earth.Manager.Instance.zoomLevel,
                mapPosition
            );

            string[] parameters = new string[5];
            string[] parametersNames = new string[5];

            parameters[0] = gpsBoundaries[0].ToString().Replace(',', '.');
            parametersNames[0] = "startLatitude";

            parameters[1] = gpsBoundaries[1].ToString().Replace(',', '.');
            parametersNames[1] = "startLongitude";

            parameters[2] = gpsBoundaries[2].ToString().Replace(',', '.');
            parametersNames[2] = "endLatitude";

            parameters[3] = gpsBoundaries[3].ToString().Replace(',', '.');
            parametersNames[3] = "endLongitude";

            return manager.url.Interpolate(parameters, parametersNames);
        }
    }
}
