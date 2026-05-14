
using Frostline.Core;
using UnityEngine;

namespace Frostline.World.Structures
{
    public class StructureBlueprint : ScriptableObject
    {
        public GameObject prefab;
        public Vector2Int[] OccupiedOffsets;
        public Vector2Int[] BoundOffsets;
        public string Name;

        public static StructureBlueprint New(GameObject prefab, string name, Vector2Int[] offsets, Vector2Int[] boundOffsets)
        {
            StructureBlueprint sb = CreateInstance<StructureBlueprint>();
            sb.OccupiedOffsets = offsets;
            sb.BoundOffsets = boundOffsets;
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
        public Vector2Int[] ToWorldPositions(Vector2Int worldPosition, Rotation rotation)
        {
            Vector2Int[] worldPositions = new Vector2Int[OccupiedOffsets.Length];
            for (int i = 0; i < OccupiedOffsets.Length; i++)
            {
                worldPositions[i] = RotationManager.Rotate(OccupiedOffsets[i], rotation) + worldPosition;
            }

            return worldPositions;
        }

        public Vector2Int[] BoundsToWorldPositions(Vector2Int worldPosition)
        {
            Vector2Int[] worldPositions = new Vector2Int[BoundOffsets.Length];
            for (int i = 0; i < BoundOffsets.Length; i++)
            {
                worldPositions[i] = BoundOffsets[i] + worldPosition;
            }

            return worldPositions;
        }
        public Vector2Int[] BoundsToWorldPositions(Vector2Int worldPosition, Rotation rotation)
        {
            Vector2Int[] worldPositions = new Vector2Int[BoundOffsets.Length];
            for (int i = 0; i < BoundOffsets.Length; i++)
            {
                worldPositions[i] = RotationManager.Rotate(BoundOffsets[i], rotation) + worldPosition;
            }

            return worldPositions;
        }
    }
}