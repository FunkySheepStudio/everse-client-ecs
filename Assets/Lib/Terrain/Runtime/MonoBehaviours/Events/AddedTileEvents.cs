using UnityEngine;

namespace FunkySheep.Earth.Terrain
{
    [CreateAssetMenu(menuName = "FunkySheep/Earth/Terrain/Added Tile Event")]
    public class AddedTileEvent : FunkySheep.Events.Event<Tile>
    {
    }
}