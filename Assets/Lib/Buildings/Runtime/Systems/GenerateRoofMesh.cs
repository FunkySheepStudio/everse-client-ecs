using System.Collections.Generic;
using TreeEditor;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using static FunkySheep.Earth.Buildings.GenerateRoofMesh;

namespace FunkySheep.Earth.Buildings
{
    public partial class GenerateRoofMesh : SystemBase
    {
        public class Vertex
        {
            public Vector3 position;
            public Vertex prevVertex;
            public Vertex nextVertex;
            //Properties this vertex may have
            //Reflex is concave
            public bool isReflex;
            public bool isConvex;
            public bool isEar;
            public int index;

            public Vertex(Vector3 position)
            {
                this.position = position;
            }

            //Get 2d pos of this vertex
            public Vector2 GetPos2D_XZ()
            {
                Vector2 pos_2d_xz = new Vector2(position.x, position.z);

                return pos_2d_xz;
            }
        }

        public class Triangle
        {
            //Corners
            public Vertex v1;
            public Vertex v2;
            public Vertex v3;
            public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
            {
                this.v1 = new Vertex(v1);
                this.v2 = new Vertex(v2);
                this.v3 = new Vertex(v3);
            }
        }

        public Material material;

        protected override void OnCreate()
        {
            material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        }


        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref DynamicBuffer<Point> floatPoints, in BuildingComponent buildingComponent, in RoofTag roofTag) =>
            {
                //The list with triangles the method returns
                List<int> triangles = new List<int>();
                List<Vector3> points = new List<Vector3>();
                List<Vector3> uvs = new List<Vector3>();

                Vector3 minPoint = floatPoints[0].Value;
                Vector3 maxPoint = floatPoints[0].Value;

                for (int i = 0; i < floatPoints.Length; i++)
                {
                    points.Add(floatPoints[i].Value);

                    if (Vector3.Distance(minPoint, floatPoints[i].Value) > Vector3.Distance(minPoint, maxPoint))
                        maxPoint = floatPoints[i].Value;
                }

                if (points.Count < 3)
                {
                    return;
                }

                //If we just have three points, then we dont have to do all calculations
                if (points.Count == 3)
                {
                    triangles.Add(0);
                    triangles.Add(1);
                    triangles.Add(2);

                    return;
                }

                //Step 1. Store the vertices in a list and we also need to know the next and prev vertex
                List<Vertex> vertices = new List<Vertex>();

                for (int i = 0; i < points.Count; i++)
                {
                    vertices.Add(new Vertex(points[i]));
                    vertices[i].index = i;

                    uvs.Add(new Vector3(
                        (points[i].x - minPoint.x) / (maxPoint.x - minPoint.x),
                        (points[i].y - minPoint.y) / (maxPoint.y - minPoint.y),
                        0
                    ));
                }

                //Find the next and previous vertex
                for (int i = 0; i < vertices.Count; i++)
                {
                    int nextPos = ClampListIndex(i + 1, vertices.Count);

                    int prevPos = ClampListIndex(i - 1, vertices.Count);

                    vertices[i].prevVertex = vertices[prevPos];

                    vertices[i].nextVertex = vertices[nextPos];
                }



                //Step 2. Find the reflex (concave) and convex vertices, and ear vertices
                for (int i = 0; i < vertices.Count; i++)
                {
                    CheckIfReflexOrConvex(vertices[i]);
                }

                //Have to find the ears after we have found if the vertex is reflex or convex
                List<Vertex> earVertices = new List<Vertex>();

                for (int i = 0; i < vertices.Count; i++)
                {
                    IsVertexEar(vertices[i], vertices, earVertices);
                }

                //Step 3. Triangulate!
                while (true)
                {
                    //This means we have just one triangle left
                    if (vertices.Count == 3)
                    {
                        //The final triangle
                        triangles.Add(vertices[0].index);
                        triangles.Add(vertices[0].prevVertex.index);
                        triangles.Add(vertices[0].nextVertex.index);
                        break;
                    }

                    if (earVertices.Count == 0)
                        break;

                    //Make a triangle of the first ear
                    Vertex earVertex = earVertices[0];

                    Vertex earVertexPrev = earVertex.prevVertex;
                    Vertex earVertexNext = earVertex.nextVertex;

                    triangles.Add(earVertex.index);
                    triangles.Add(earVertexPrev.index);
                    triangles.Add(earVertexNext.index);

                    //Remove the vertex from the lists
                    earVertices.Remove(earVertex);

                    vertices.Remove(earVertex);

                    //Update the previous vertex and next vertex
                    earVertexPrev.nextVertex = earVertexNext;
                    earVertexNext.prevVertex = earVertexPrev;

                    //...see if we have found a new ear by investigating the two vertices that was part of the ear
                    CheckIfReflexOrConvex(earVertexPrev);
                    CheckIfReflexOrConvex(earVertexNext);

                    earVertices.Remove(earVertexPrev);
                    earVertices.Remove(earVertexNext);

                    IsVertexEar(earVertexPrev, vertices, earVertices);
                    IsVertexEar(earVertexNext, vertices, earVertices);
                }

                triangles.Reverse();
                //debugTriangles(triangles, points);

                Mesh mesh = new Mesh();
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                mesh.indexFormat = IndexFormat.UInt32;
                mesh.Clear();
                mesh.SetVertices(points.ToArray());
                mesh.SetIndices(triangles.ToArray(), MeshTopology.Triangles, 0);
                mesh.SetUVs(0, uvs.ToArray());
                mesh.RecalculateNormals();

                var desc = new RenderMeshDescription(
                    shadowCastingMode: ShadowCastingMode.Off,
                    receiveShadows: false);

                // Create an array of mesh and material required for runtime rendering.
                var renderMeshArray = new RenderMeshArray(new Material[] { material }, new Mesh[] { mesh });

                //var prototype = entityManager.CreateEntity();

                // Call AddComponents to populate base entity with the components required
                // by Entities Graphics
                RenderMeshUtility.AddComponents(
                    entity,
                    entityManager,
                    desc,
                    renderMeshArray,
                    MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));

                entityManager.AddComponent<LocalToWorldTransform>(entity);
                entityManager.SetComponentData<LocalToWorldTransform>(entity, new LocalToWorldTransform
                {
                    Value = new UniformScaleTransform
                    {
                        Scale = 1
                    }
                });

                entityManager.SetComponentEnabled<RoofTag>(entity, false);
            })
            .WithStructuralChanges()
            .WithoutBurst()
            .Run();
        }

        public void debugTriangles(List<int> triangles, List<Vector3> points)
        {
            for (int i = 0; i < triangles.Count; i+=3)
            {
                UnityEngine.Debug.DrawLine(points[triangles[i]], points[triangles[i + 1]], Color.red, 10000);
                UnityEngine.Debug.DrawLine(points[triangles[i + 1]], points[triangles[i + 2]], Color.red, 10000);
                UnityEngine.Debug.DrawLine(points[triangles[i + 2]], points[triangles[i]], Color.red, 10000);
            }
        }

        //Clamp list indices
        //Will even work if index is larger/smaller than listSize, so can loop multiple times
        public static int ClampListIndex(int index, int listSize)
        {
            index = ((index % listSize) + listSize) % listSize;

            return index;
        }


        //Check if a vertex if reflex or convex, and add to appropriate list
        private static void CheckIfReflexOrConvex(Vertex v)
        {
            v.isReflex = false;
            v.isConvex = false;

            //This is a reflex vertex if its triangle is oriented clockwise
            Vector2 a = v.prevVertex.GetPos2D_XZ();
            Vector2 b = v.GetPos2D_XZ();
            Vector2 c = v.nextVertex.GetPos2D_XZ();

            if (!IsTriangleOrientedClockwise(a, b, c))
            {
                v.isReflex = true;
            }
            else
            {
                v.isConvex = true;
            }
        }

        //Is a triangle in 2d space oriented clockwise or counter-clockwise
        //https://math.stackexchange.com/questions/1324179/how-to-tell-if-3-connected-points-are-connected-clockwise-or-counter-clockwise
        //https://en.wikipedia.org/wiki/Curve_orientation
        public static bool IsTriangleOrientedClockwise(float2 p1, float2 p2, float2 p3)
        {
            bool isClockWise = true;

            float determinant = p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;

            if (determinant > 0f)
            {
                isClockWise = false;
            }

            return isClockWise;
        }

        //Check if a vertex is an ear
        private static void IsVertexEar(Vertex v, List<Vertex> vertices, List<Vertex> earVertices)
        {
            //A reflex vertex cant be an ear!
            if (v.isReflex)
            {
                return;
            }

            //This triangle to check point in triangle
            Vector2 a = v.prevVertex.GetPos2D_XZ();
            Vector2 b = v.GetPos2D_XZ();
            Vector2 c = v.nextVertex.GetPos2D_XZ();

            bool hasPointInside = false;

            for (int i = 0; i < vertices.Count; i++)
            {
                //We only need to check if a reflex vertex is inside of the triangle
                if (vertices[i].isReflex)
                {
                    Vector2 p = vertices[i].GetPos2D_XZ();

                    //This means inside and not on the hull
                    if (IsPointInTriangle(a, b, c, p))
                    {
                        hasPointInside = true;

                        break;
                    }
                }
            }

            if (!hasPointInside)
            {
                earVertices.Add(v);
            }
        }

        //From http://totologic.blogspot.se/2014/01/accurate-point-in-triangle-test.html
        //p is the testpoint, and the other points are corners in the triangle
        public static bool IsPointInTriangle(float2 p1, float2 p2, float2 p3, float2 p)
        {
            bool isWithinTriangle = false;

            //Based on Barycentric coordinates
            float denominator = ((p2.y - p3.y) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.y - p3.y));

            float a = ((p2.y - p3.y) * (p.x - p3.x) + (p3.x - p2.x) * (p.y - p3.y)) / denominator;
            float b = ((p3.y - p1.y) * (p.x - p3.x) + (p1.x - p3.x) * (p.y - p3.y)) / denominator;
            float c = 1 - a - b;

            //The point is within the triangle or on the border if 0 <= a <= 1 and 0 <= b <= 1 and 0 <= c <= 1
            //if (a >= 0f && a <= 1f && b >= 0f && b <= 1f && c >= 0f && c <= 1f)
            //{
            //    isWithinTriangle = true;
            //}

            //The point is within the triangle
            if (a > 0f && a < 1f && b > 0f && b < 1f && c > 0f && c < 1f)
            {
                isWithinTriangle = true;
            }

            return isWithinTriangle;
        }

    }
}
