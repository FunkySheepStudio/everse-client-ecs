using Unity.Entities;
using Unity.Collections;
public partial class AddTile : SystemBase
{
    public NativeQueue<TileComponent> addedTiles;

    protected override void OnCreate()
    {
        addedTiles = new NativeQueue<TileComponent>(Allocator.Persistent);
    }

    protected override void OnUpdate()
    {
    }

    protected override void OnDestroy()
    {
        addedTiles.Dispose();
    }
}
