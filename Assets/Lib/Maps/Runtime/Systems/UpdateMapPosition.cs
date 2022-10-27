using FunkySheep.Earth;
using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Maps
{
    public partial class UpdateMapPosition : SystemBase
    {
        protected override void OnUpdate()
        {
            InitialMapPosition initialMapPosition;
            if (!TryGetSingleton<InitialMapPosition>(out initialMapPosition))
                return;

            Entities.ForEach((ref MapPosition mapPosition, in GridPosition gridPosition, in GridPositionChangedTag gridPositionChangedTag) =>
            {
                mapPosition.Value = new float2
                {
                    x = initialMapPosition.Value.x + gridPosition.Value.x,
                    y = initialMapPosition.Value.y - gridPosition.Value.y,
                };
            })
            .WithoutBurst()
            .Run();
        }
    }
}
