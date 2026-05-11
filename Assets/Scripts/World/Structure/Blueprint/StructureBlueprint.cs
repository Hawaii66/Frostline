
using UnityEngine;

namespace Frostline.World.Structures
{
    public class StructureBlueprint : ScriptableObject
    {
        public Vector2Int[] OccupiedOffsets;

        public static StructureBlueprint New(Vector2Int[] offsets)
        {
            StructureBlueprint sb = CreateInstance<StructureBlueprint>();
            sb.OccupiedOffsets = offsets;
            return sb;
        }

        public Vector2Int[] ToWorldPositions(Vector2Int worldPosition)
        {
            Vector2Int[] worldPositions = new Vector2Int[OccupiedOffsets.Length];
            for (int i = 0; i < OccupiedOffsets.Length; i++)
            {
                worldPositions[i] = OccupiedOffsets[i] + worldPosition;
            }

            return worldPositions;
        }
    }
}