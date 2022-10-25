using FunkySheep.Earth;
using Unity.Entities;

namespace FunkySheep.Terrain
{
    public struct Tile : IBufferElementData
    {
        public GridPosition position;
    }
}
