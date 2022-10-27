using FunkySheep.Types;
using FunkySheep.Earth;
using Unity.Entities;

namespace Game.Player
{
    public class Manager : Singleton<Manager>
    {
        public GPSCoordinates GPSCoordinates;
        public GridCoordinates gridCoordinates;

        GridCoordinates _lastGridCoordinates;

        private void Start()
        {
            FunkySheep.Earth.Manager.Instance.Init(GPSCoordinates);
            FunkySheep.Earth.Terrain.Manager.Instance.AddTile(gridCoordinates.Value);
        }

        private void Update()
        {
            gridCoordinates.Value = FunkySheep.Earth.Manager.GetTilePosition(transform.position);
            if (!_lastGridCoordinates.Value.Equals(gridCoordinates.Value))
            {
                FunkySheep.Earth.Terrain.Manager.Instance.AddTile(gridCoordinates.Value);

                _lastGridCoordinates.Value = gridCoordinates.Value;
            }
        }
    }

    public class PlayerManagerBaker : Baker<Manager>
    {
        public override void Bake(Manager authoring)
        {
            AddComponent<GPSCoordinates>(authoring.GPSCoordinates);
        }
    }
}
