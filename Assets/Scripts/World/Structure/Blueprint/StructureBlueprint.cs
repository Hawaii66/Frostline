
using UnityEngine;

namespace Frostline.World.Structures
{
    public class StructureBlueprint : ScriptableObject
    {
        public GameObject prefab;
        public Vector2Int[] OccupiedOffsets;
        public string Name;

        public static StructureBlueprint New(GameObject prefab, string name, Vector2Int[] offsets)
        {
            StructureBlueprint sb = CreateInstance<StructureBlueprint>();
            sb.OccupiedOffsets = offsets;
            sb.prefab = prefab;
            sb.Name = name;
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