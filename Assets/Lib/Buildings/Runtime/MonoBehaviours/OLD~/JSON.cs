using Unity.Mathematics;
using Unity.Entities;
using System;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

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
        public JsonOsmElement[] members;

        public GPSCoordinatesArray[] AsGPSCoordinatesArray()
        {
            return Array.ConvertAll(geometry, item => new GPSCoordinatesArray { Value = item.coordinates });
        }
    }

    public struct Point : IBufferElementData
    {
        public float3 Value;
        //Get 2d pos of this vertex
        public float2 GetPos2D_XZ()
        {
            float2 pos_2d_xz = new float2(Value.x, Value.z);

            return pos_2d_xz;
        }
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