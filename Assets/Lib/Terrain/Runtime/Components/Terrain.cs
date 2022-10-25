using Unity.Entities;
namespace FunkySheep.Terrain
{
    public struct Terrain : IComponentData
    {
        public DynamicBuffer<Tile> tiles;
    }
}
