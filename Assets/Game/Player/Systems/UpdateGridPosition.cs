using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Game.Player
{
    public partial class UpdateGridPosition : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref CurrentGridPosition currentGridPosition, in LocalToWorldTransform localToWorldTransform) =>
            {
                currentGridPosition.Value = new Unity.Mathematics.int2
                {
                    x = 20 * (int)math.floor(localToWorldTransform.Value.Position.x / 20),
                    y = 20 * (int)math.floor(localToWorldTransform.Value.Position.z / 20),
                };
            }).ScheduleParallel();
        }
    }
}

