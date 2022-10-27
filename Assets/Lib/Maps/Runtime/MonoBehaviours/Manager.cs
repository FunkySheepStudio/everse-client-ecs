using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using FunkySheep.Types;

namespace FunkySheep.Maps
{
    [AddComponentMenu("FunkySheep/Maps/Manager")]
    public class Manager : Singleton<Manager>
    {
        public FunkySheep.Types.Int32 zoomLevel;
        EntityManager entityManager;
        public override void Awake()
        {
            base.Awake();
            Instance.entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void Start()
        {
            Entity zoomLevelSingleton = entityManager.CreateEntity();
            entityManager.SetName(zoomLevelSingleton, new FixedString64Bytes("ZoomLevel"));
            entityManager.AddComponent<ZoomLevel>(zoomLevelSingleton);
            entityManager.SetComponentData<ZoomLevel>(zoomLevelSingleton, new ZoomLevel
            {
                Value = zoomLevel.value
            });
        }
    }
}
