using Unity.Mathematics;
using Unity.Entities;

namespace FunkySheep.Earth.Buildings
{
    [System.Serializable]
    public struct JsonOsmRoot
    {
        public JsonOsmElement[] elements;
    }

    [System.Serializable]
    public struct JsonOsmElement
    {
        public string type;
        public JsonOsmGeometry[] geometry;

        /*public float3[] points
        {
            get {
                float3[] points = new float3[geometry.Length];

                for (int i = 0; i < points.Length; i++)
                {
                    points[i] = Earth.Manager.GetWorldPosition(geometry[i].coordinates);
                    points[i].y = UnityEngine.Terrain.activeTerrain.SampleHeight(points[i]);
                }

                return points;
            }
        }*/

        /*public DynamicBuffer<Point> points
        {
            get
            {
                DynamicBuffer<Point> points = new DynamicBuffer<Point>();

                for (int i = 0; i < geometry.Length; i++)
                {
                    float3 point = Earth.Manager.GetWorldPosition(geometry[i].coordinates);
                    point.y = UnityEngine.Terrain.activeTerrain.SampleHeight(point);

                    points.Add(
                        new Point
                        {
                            Value = point
                        }
                    );
                }

                return points;
            }
        }*/
    }

    public struct Point : IBufferElementData
    {
        public float3 Value;
    }

    [System.Serializable]
    public struct JsonOsmGeometry
    {
        public double lat;
        public double lon;
        public GPSCoordinates coordinates
        {
            get {
                return new GPSCoordinates { Value = new double2(lat, lon) };
            }
        }
    }
}