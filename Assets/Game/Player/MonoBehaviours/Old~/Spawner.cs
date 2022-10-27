using UnityEngine;
using Unity.Mathematics;

namespace Game.Player
{
    [AddComponentMenu("Game/Player/Spawner")]
    [RequireComponent(typeof(Manager))]
    public class Spawner : MonoBehaviour
    {
        Manager manager;
        private void Awake()
        {
            manager = GetComponent<Manager>();
        }

        private void Update()
        {
            float3 point = FunkySheep.Earth.Manager.GetWorldPosition(manager.GPSCoordinates);
            float? height = FunkySheep.Earth.Terrain.Manager.GetHeight(point);
            if (height == null)
            {
                return;
            }
            else
            {
                transform.position = new Vector3(
                    point.x,
                    height.Value,
                    point.z
                );

                enabled = false;
            }
        }
    }
}
