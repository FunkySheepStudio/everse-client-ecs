using FunkySheep.Earth;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;


namespace FunkySheep.Terrain
{
    public partial class DebugSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            
        }
        public void OnDrawGizmos()
        {
            /*Entities.ForEach((Entity entity, int entityInQueryIndex, in LocalToWorldTransform localToWorldTransform, in DebugTag debugTag, in Lod0Tag lod0Tag) =>
            {
                if (entity.Index%4 != 0 || (int)math.floor(entity.Index / 256)% 4 != 0)
                    return;
                Gizmos.color = Color.red;
                Gizmos.DrawCube(localToWorldTransform.Value.Position, Vector3.one * 5);
            })
            .WithoutBurst()
            .Run();

            Entities.ForEach((Entity entity, int entityInQueryIndex, in LocalToWorldTransform localToWorldTransform, in DebugTag debugTag, in Lod1Tag lod1Tag) =>
            {
                if (entity.Index%8 != 0 || (int)math.floor(entity.Index / 256)% 8 != 0)
                    return;
                Gizmos.color = Color.green;
                Gizmos.DrawCube(localToWorldTransform.Value.Position, Vector3.one * 10);
            })
            .WithoutBurst()
            .Run();

            Entities.ForEach((Entity entity, int entityInQueryIndex, in LocalToWorldTransform localToWorldTransform, in DebugTag debugTag, in Lod2Tag lod2Tag) =>
            {
                if (entity.Index%16 != 0 || (int)math.floor(entity.Index / 256)% 16 != 0)
                    return;
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(localToWorldTransform.Value.Position, Vector3.one * 20);
            })
            .WithoutBurst()
            .Run();*/

            /*Entities.ForEach((Entity entity, int entityInQueryIndex, in LocalToWorldTransform localToWorldTransform, in DebugTag debugTag, in TopEntity topEntity) =>
            {
                LocalToWorldTransform topEntityPosition;
                if (!GetComponentLookup<LocalToWorldTransform>(true).TryGetComponent(topEntity.Value, out topEntityPosition))
                    return;

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(localToWorldTransform.Value.Position, topEntityPosition.Value.Position);
            })
            .WithoutBurst()
            .Run();

            Entities.ForEach((Entity entity, int entityInQueryIndex, in LocalToWorldTransform localToWorldTransform, in DebugTag debugTag, in BottomEntity bottomEntity) =>
            {
                LocalToWorldTransform bottomEntityPosition;
                if (!GetComponentLookup<LocalToWorldTransform>(true).TryGetComponent(bottomEntity.Value, out bottomEntityPosition))
                    return;

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(localToWorldTransform.Value.Position, bottomEntityPosition.Value.Position);
            })
            .WithoutBurst()
            .Run();

            Entities.ForEach((Entity entity, int entityInQueryIndex, in LocalToWorldTransform localToWorldTransform, in DebugTag debugTag, in RightEntity rightEntity) =>
            {
                LocalToWorldTransform rightEntityPosition;
                if (!GetComponentLookup<LocalToWorldTransform>(true).TryGetComponent(rightEntity.Value, out rightEntityPosition))
                    return;

                Gizmos.color = Color.red;
                Gizmos.DrawLine(localToWorldTransform.Value.Position, rightEntityPosition.Value.Position);
            })
            .WithoutBurst()
            .Run();

            Entities.ForEach((Entity entity, int entityInQueryIndex, in LocalToWorldTransform localToWorldTransform, in DebugTag debugTag, in LeftEntity leftEntity) =>
            {
                LocalToWorldTransform leftEntityPosition;
                if (!GetComponentLookup<LocalToWorldTransform>(true).TryGetComponent(leftEntity.Value, out leftEntityPosition))
                    return;

                Gizmos.color = Color.green;
                Gizmos.DrawLine(localToWorldTransform.Value.Position, leftEntityPosition.Value.Position);
            })
            .WithoutBurst()
            .Run();*/

            Entities.ForEach((Entity entity, int entityInQueryIndex, in LocalToWorldTransform localToWorldTransform, in DebugTag debugTag, in LodBorder lodBorder) =>
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(localToWorldTransform.Value.Position, Vector3.one * 2);
            })
            .WithoutBurst()
            .Run();
        }
    }
}
