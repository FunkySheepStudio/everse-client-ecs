using Unity.Entities;
using UnityEngine;

namespace FunkySheep.Terrain
{
    public class Debug : MonoBehaviour
    {
        DebugSystem system;

        private void Start()
        {
            system = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<DebugSystem>();
        }

        private void OnDrawGizmos()
        {
            if (system != null)
            {
                system.OnDrawGizmos();
            }
        }
    }
}
