using System;
using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;

namespace FunkySheep.Earth
{
    [BurstCompile]
    public static class Utils
    {
        private static readonly double R_MAJOR = 6378137.0;
        private static readonly double R_MINOR = 6356752.3142;
        private static readonly double RATIO = R_MINOR / R_MAJOR;
        private static readonly double ECCENT = Math.Sqrt(1.0 - (RATIO * RATIO));
        private static readonly double COM = 0.5 * ECCENT;

        private static readonly double DEG2RAD = Math.PI / 180.0;
        private static readonly double RAD2Deg = 180.0 / Math.PI;
        private static readonly double PI_2 = Math.PI / 2.0;

        public static double[] toCartesianArray(double lon, double lat)
        {
            return new double[] { lonToX(lon), latToY(lat) };
        }

        public static Vector3 toCartesianVector(double lon, double lat)
        {
            return new Vector3((float)lonToX(lon), 0, (float)latToY(lat));
        }

        public static Vector2 toCartesianVector2(double lon, double lat)
        {
            return new Vector2((float)lonToX(lon), (float)latToY(lat));
        }

        public static Vector2 toCartesianFloat2(double2 gps)
        {
            return new float2((float)lonToX(gps.y), (float)latToY(gps.x));
        }

        public static double[] toGps(double x, double y)
        {
            return new double[] { xToLon(x), yToLat(y) };
        }

        public static (double latitude, double longitude) toGeoCoord(Vector2 position)
        {
            return (yToLat(position.y), xToLon(position.x));
        }

        public static double2 toGeoCoordDouble2(float2 position)
        {
            return new double2
            {
                x = yToLat(position.y),
                y = xToLon(position.x)
            };
        }

        public static double lonToX(double lon)
        {
            return R_MAJOR * DegToRad(lon);
        }

        public static double latToY(double lat)
        {
            lat = Math.Min(89.5, Math.Max(lat, -89.5));
            double phi = DegToRad(lat);
            double sinphi = Math.Sin(phi);
            double con = ECCENT * sinphi;
            con = Math.Pow(((1.0 - con) / (1.0 + con)), COM);
            double ts = Math.Tan(0.5 * ((Math.PI * 0.5) - phi)) / con;
            return 0 - R_MAJOR * Math.Log(ts);
        }

        public static double xToLon(double x)
        {
            return RadToDeg(x) / R_MAJOR;
        }

        public static double yToLat(double y)
        {
            double ts = Math.Exp(-y / R_MAJOR);
            double phi = PI_2 - 2 * Math.Atan(ts);
            double dphi = 1.0;
            int i = 0;
            while ((Math.Abs(dphi) > 0.000000001) && (i < 15))
            {
                double con = ECCENT * Math.Sin(phi);
                dphi = PI_2 - 2 * Math.Atan(ts * Math.Pow((1.0 - con) / (1.0 + con), COM)) - phi;
                phi += dphi;
                i++;
            }
            return RadToDeg(phi);
        }

        private static double RadToDeg(double rad)
        {
            return rad * RAD2Deg;
        }

        private static double DegToRad(double deg)
        {
            return deg * DEG2RAD;
        }


        /// <summary>
        /// Get the map tile position depending on zoom level and GPS postions
        /// </summary>
        /// <returns></returns>
        public static Vector2Int GpsToMap(int zoom, double latitude, double longitude)
        {
            return new Vector2Int(
                LongitudeToX(zoom, longitude),
                LatitudeToZ(zoom, latitude)
            );
        }

        public static int2 GpsToMapInt2(int zoom, double latitude, double longitude)
        {
            return new int2(
                LongitudeToX(zoom, longitude),
                LatitudeToZ(zoom, latitude)
            );
        }

        /// <summary>
        /// Get the map tile position depending on zoom level and GPS postions
        /// https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#Lon..2Flat._to_tile_numbers
        /// </summary>
        /// <returns></returns>
        public static Vector2 GpsToMapReal(int zoom, double latitude, double longitude)
        {
            Vector2 p = new Vector2();
            p.x = (float)((longitude + 180.0) / 360.0 * (1 << zoom));
            p.y = (float)((1.0 - Math.Log(Math.Tan(latitude * Math.PI / 180.0) +
              1.0 / Math.Cos(latitude * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));

            return p;
        }

        /// <summary>
        /// Get the map tile position depending on zoom level and GPS postions
        /// https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#Lon..2Flat._to_tile_numbers
        /// </summary>
        /// <returns></returns>
        public static Vector2 GpsToMapReal(int zoom, double2 gps)
        {
            return GpsToMapReal(zoom, gps.x, gps.y);
        }

        /// <summary>
        /// Get the map tile position depending on zoom level and GPS postions
        /// https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#Lon..2Flat._to_tile_numbers
        /// </summary>
        /// <returns></returns>
        public static Vector2 GpsToMapRealFloat2(int zoom, double2 gps)
        {
            Vector2 mapCoordinates = GpsToMapReal(zoom, gps.x, gps.y);
            return new float2(mapCoordinates.x, mapCoordinates.y);
        }

        /// <summary>
        /// Get the map tile position depending on zoom level and GPS postions
        /// https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#Lon..2Flat._to_tile_numbers
        /// </summary>
        /// <returns></returns>
        public static Vector2 GpsToMapReal(int zoom, double latitude, double longitude, Vector2 offset)
        {
            Vector2 p = new Vector2();
            p.x = (float)(((longitude + 180.0) / 360.0 * (1 << zoom)) - offset.x);
            p.y = (float)(((1.0 - Math.Log(Math.Tan(latitude * Math.PI / 180.0) +
              1.0 / Math.Cos(latitude * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom)) - offset.y);

            return p;
        }

        /// <summary>
        /// Get the X number of the tile relative to Longitude position
        /// </summary>
        /// <returns></returns>
        public static int LongitudeToX(int zoom, double longitude)
        {
            return (int)(Math.Floor((longitude + 180.0) / 360.0 * (1 << zoom)));
        }

        /// <summary>
        /// Get the X number of the tile relative to Longitude position
        /// </summary>
        /// <returns></returns>
        public static float LongitudeToXReal(int zoom, double longitude)
        {
            return (float)((longitude + 180.0) / 360.0 * (1 << zoom));
        }


        /// <summary>
        /// Get the Real X number inside the tile
        /// </summary>
        /// <returns></returns>
        public static float LongitudeToInsideX(int zoom, double longitude)
        {
            return (float)((longitude + 180.0) / 360.0 * (1 << zoom) - LongitudeToX(zoom, longitude));
        }

        /// <summary>
        /// /// Get the Y number of the tile relative to Latitude position
        /// /// !!! The Y position is the reverse of the cartesian one !!!
        /// </summary>
        /// <returns></returns>
        public static int LatitudeToZ(int zoom, double latitude)
        {
            return (int)Math.Floor((1 - Math.Log(Math.Tan(Mathf.Deg2Rad * latitude) + 1 / Math.Cos(Mathf.Deg2Rad * latitude)) / Math.PI) / 2 * (1 << zoom));
        }

        /// <summary>
        /// /// Get the Y number of the tile relative to Latitude position
        /// /// !!! The Y position is the reverse of the cartesian one !!!
        /// </summary>
        /// <returns></returns>
        public static float LatitudeToZReal(int zoom, double latitude)
        {
            return (float)((1 - Math.Log(Math.Tan(Mathf.Deg2Rad * latitude)) / Math.PI) / 2 * (1 << zoom));
        }

        /// <summary>
        /// /// Get the Real Y number inside of the tile
        /// </summary>
        /// <returns></returns>
        public static float LatitudeToInsideZ(int zoom, double latitude)
        {
            return (float)((1 - Math.Log(Math.Tan(Mathf.Deg2Rad * latitude) + 1 / Math.Cos(Mathf.Deg2Rad * latitude)) / Math.PI) / 2 * (1 << zoom)) - LatitudeToZ(zoom, latitude);
        }

        /// <summary>
        /// Get the Longitude of the tile relative to X position
        /// </summary>
        /// <returns></returns>
        public static double tileX2long(int zoom, float xPosition)
        {
            return xPosition / (double)(1 << zoom) * 360.0 - 180;
        }

        /// <summary>
        ///  Get the latitude of the tile relative to Y position
        /// </summary>
        /// <returns></returns>
        public static double tileZ2lat(int zoom, float zposition)
        {
            double n = Math.PI - 2.0 * Math.PI * zposition / (double)(1 << zoom);
            return 180.0 / Math.PI * Math.Atan(0.5 * (Math.Exp(n) - Math.Exp(-n)));
        }

        /// <summary>
        /// Calculate size of the OSM tile depending on the zoomValue level and latitude
        /// </summary>
        /// <returns></returns>
        public static double TileSize(int zoom, double latitude)
        {
            return 156543.03 / Math.Pow(2, zoom) * Math.Cos(Mathf.Deg2Rad * latitude) * 256;
        }

        /// <summary>
        /// Calculate size of the OSM tile depending on the zoomValue level.
        /// </summary>
        /// <returns></returns>
        public static double TileSize(int zoom)
        {
            return 156543.03 / Math.Pow(2, zoom) * 256;
        }

        /// <summary>
        /// Calculate the GPS boundaries of a tile depending on zoom size
        /// </summary>
        /// <returns>A Double[4] containing [StartLatitude, StartLongitude, EndLatitude, EndLongitude]</returns>
        public static Double[] CaclulateGpsBoundaries(int zoom, double latitude, double longitude)
        {
            Vector2Int mapPosition = GpsToMap(zoom, latitude, longitude);

            double startlatitude = Utils.tileZ2lat(zoom, mapPosition.y + 1);
            double startlongitude = Utils.tileX2long(zoom, mapPosition.x);
            double endLatitude = Utils.tileZ2lat(zoom, mapPosition.y);
            double endLongitude = Utils.tileX2long(zoom, mapPosition.x + 1);

            Double[] boundaries = new Double[4];

            boundaries[0] = startlatitude;
            boundaries[1] = startlongitude;
            boundaries[2] = endLatitude;
            boundaries[3] = endLongitude;

            return boundaries;
        }

        /// <summary>
        /// Calculate the GPS boundaries of a tile depending on zoom size and the position on the map
        /// </summary>
        /// <returns>A Double[4] containing [StartLatitude, StartLongitude, EndLatitude, EndLongitude]</returns>
        public static Double[] CaclulateGpsBoundaries(int zoom, Vector2Int mapPosition)
        {
            double startlatitude = Utils.tileZ2lat(zoom, mapPosition.y + 1);
            double startlongitude = Utils.tileX2long(zoom, mapPosition.x);
            double endLatitude = Utils.tileZ2lat(zoom, mapPosition.y);
            double endLongitude = Utils.tileX2long(zoom, mapPosition.x + 1);

            Double[] boundaries = new Double[4];

            boundaries[0] = startlatitude;
            boundaries[1] = startlongitude;
            boundaries[2] = endLatitude;
            boundaries[3] = endLongitude;

            return boundaries;
        }

        /// <summary>
        /// Calculate the GPS boundaries of a tile depending on zoom size and the position on the map
        /// </summary>
        /// <returns>A Double[4] containing [StartLatitude, StartLongitude, EndLatitude, EndLongitude]</returns>
        public static Double[] CaclulateGpsBoundaries(int zoom, int2 mapPosition)
        {
            double startlatitude = Utils.tileZ2lat(zoom, mapPosition.y + 1);
            double startlongitude = Utils.tileX2long(zoom, mapPosition.x);
            double endLatitude = Utils.tileZ2lat(zoom, mapPosition.y);
            double endLongitude = Utils.tileX2long(zoom, mapPosition.x + 1);

            Double[] boundaries = new Double[4];

            boundaries[0] = startlatitude;
            boundaries[1] = startlongitude;
            boundaries[2] = endLatitude;
            boundaries[3] = endLongitude;

            return boundaries;
        }

        /// <summary>
        /// Convert from color channel values in 0.0-1.0 range to elevation in meters:
        /// 21768
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static float ColorToElevation(Color color)
        {
            float height = (Mathf.Floor(color.r * 256.0f) * 256.0f + Mathf.Floor(color.g * 256.0f) + color.b) - 32768.0f;
            return height;
        }

    }
}
