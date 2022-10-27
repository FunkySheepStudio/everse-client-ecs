using Unity.Entities;
using Unity.Transforms;

namespace FunkySheep.Transforms
{
    public struct LastLocalToWorldTransform : IComponentData
    {
        public LocalToWorldTransform Value;
    }
}
