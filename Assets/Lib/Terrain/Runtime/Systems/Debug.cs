using FunkySheep.Earth;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;


namespace FunkySheep.Terrain
{
    public partial class DebugSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            
        }
        public void OnDrawGizmos()
        {
            Entity player = GetSingletonEntity<SpawnerTag>();
            LocalToWorldTransform playerPosition;
            GetComponentLookup<LocalToWorldTransform>().TryGetComponent(player,out playerPosition);

            Entities.ForEach((Entity entity, int entityInQueryIndex, in LocalToWorldTransform localToWorldTransform, in DebugTag debugTag, in Lod0Tag lod0Tag) =>
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(localToWorldTransform.Value.Position, Vector3.one * 1);
            })
            .WithoutBurst()
            .Run();

            Entities.ForEach((Entity entity, int entityInQueryIndex, in LocalToWorldTransform localToWorldTransform, in DebugTag debugTag, in Lod1Tag lod1Tag) =>
            {
                if (entity.Index % 4 != 0)
                    return;
                Gizmos.color = Color.green;
                Gizmos.DrawCube(localToWorldTransform.Value.Position, Vector3.one * 1);
            })
            .WithoutBurst()
            .Run();

            Entities.ForEach((Entity entity, int entityInQueryIndex, in LocalToWorldTransform localToWorldTransform, in DebugTag debugTag, in Lod2Tag lod2Tag) =>
            {
                if (entity.Index % 8 != 0)
                    return;
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(localToWorldTransform.Value.Position, Vector3.one * 1);
            })
            .WithoutBurst()
            .Run();
        }
    }
}
