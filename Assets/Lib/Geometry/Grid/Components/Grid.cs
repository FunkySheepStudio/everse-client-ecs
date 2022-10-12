using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Geometry.Grid
{
    public struct Grid : IComponentData
    {
        public int size;
        public Entity prefab;
    }

}
