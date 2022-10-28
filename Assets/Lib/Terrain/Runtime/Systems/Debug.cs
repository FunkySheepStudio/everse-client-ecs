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
            Entities.ForEach((Entity entity, in LocalToWorldTransform localToWorldTransform, in DebugTag debugTag) =>
            {
                Gizmos.DrawCube(localToWorldTransform.Value.Position, Vector3.one);
            })
            .WithoutBurst()
            .Run();
        }
    }
}
