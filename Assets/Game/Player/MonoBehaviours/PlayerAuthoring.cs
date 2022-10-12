using Unity.Entities;
using UnityEngine;

namespace Game.Player
{
    public class PlayerAuthoring : MonoBehaviour
    {
        
    }

    public class PlayerAuthoringBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            AddComponent<CurrentGridPosition>();
        }
    }
}
