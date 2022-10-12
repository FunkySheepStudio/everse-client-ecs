using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Player
{
    public struct CurrentGridPosition : IComponentData
    {
        public int2 Value;
    }
}
