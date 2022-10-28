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

            Entities.ForEach((Entity entity, in LocalToWorldTransform localToWorldTransform, in DebugTag debugTag) =>
            {
                Vector2 player2DPosition = new Vector2(
                    playerPosition.Value.Position.x,
                    playerPosition.Value.Position.z
                );

                Vector2 pointPosition = new Vector2(
                    localToWorldTransform.Value.Position.x,
                    localToWorldTransform.Value.Position.z
                );


                if (Vector2.Distance(pointPosition, player2DPosition) < 200)
                {
                    Gizmos.color = Color.red;
                } else
                {
                    Gizmos.color = Color.blue;
                }
                Gizmos.DrawCube(localToWorldTransform.Value.Position, Vector3.one * 8);
            })
            .WithoutBurst()
            .Run();
        }
    }
}
