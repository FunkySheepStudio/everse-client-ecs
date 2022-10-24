using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using FunkySheep.Geometry;
using Unity.Collections;

namespace FunkySheep.Earth.Buildings
{
    public partial class UpdateRoofMesh : SystemBase
    {
        public Material material;

        protected override void OnCreate()
        {
            material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        }


        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, in DynamicBuffer<Uv> uvs, in DynamicBuffer<Triangle> triangles, in DynamicBuffer<Point> floatPoints, in BuildingComponent buildingComponent) =>
            {
                NativeArray<Triangle> newTriangles = new NativeArray<Triangle>(triangles.Length, Allocator.Temp);

                // reverse the array
                for (int i = 0; i < triangles.Length; i++)
                {
                    newTriangles[triangles.Length - 1 - i] = triangles[i];
                }

                Mesh mesh = new Mesh();
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                mesh.indexFormat = IndexFormat.UInt32;
                mesh.Clear();
                mesh.SetVertices(floatPoints.Reinterpret<Vector3>().AsNativeArray());
                mesh.SetIndices(newTriangles, MeshTopology.Triangles, 0);
                mesh.SetUVs(0, uvs.AsNativeArray());
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

                entityManager.RemoveComponent<Uv>(entity);
                entityManager.RemoveComponent<Triangle>(entity);
                newTriangles.Dispose();
            })
            .WithStructuralChanges()
            .WithoutBurst()
            .Run();
        }

        public void debugTriangles(List<int> triangles, List<Vector3> points)
        {
            for (int i = 0; i < triangles.Count; i+=3)
            {
                UnityEngine.Debug.DrawLine(points[triangles[i]], points[triangles[i + 1]], Color.red, 1);
                UnityEngine.Debug.DrawLine(points[triangles[i + 1]], points[triangles[i + 2]], Color.red, 1);
                UnityEngine.Debug.DrawLine(points[triangles[i + 2]], points[triangles[i]], Color.red, 1);
            }
        }

    }
}
