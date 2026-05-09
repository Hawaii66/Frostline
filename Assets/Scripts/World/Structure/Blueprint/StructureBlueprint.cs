
using UnityEngine;

class StructureBlueprint : ScriptableObject
{
    public Vector2Int[] occupiedOffsets;

    public static StructureBlueprint New(Vector2Int[] offsets)
    {
        StructureBlueprint sb = ScriptableObject.CreateInstance<StructureBlueprint>();
        sb.occupiedOffsets = offsets;
        return sb;
    }

    public Tile[] ToTiles(Vector2Int worldPosition)
    {
        Tile[] tiles = new Tile[occupiedOffsets.Length];
        for (int i = 0; i < occupiedOffsets.Length; i++)
        {
            tiles[i] = new Tile(occupiedOffsets[i] + worldPosition);
        }

        return tiles;
    }
}
