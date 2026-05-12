using Frostline.World.Structures.Editor;
using NaughtyAttributes;
using UnityEngine;

namespace Frostline.World.Structures
{
    public class StructureBlueprintMetadata : MonoBehaviour
    {
        [SerializeField] private Vector2Int[] _occupiedPositions;
        [SerializeField] private Vector2Int lowerLeftCorner;

        [Button("Generate Blueprint")]
        private void GenerateBlueprint()
        {
            StructureBlueprintGenerator.GenerateStructure(transform);
        }

        public Vector2Int[] GetOccupiedTiles()
        {
            return _occupiedPositions;
        }

        private void OnDrawGizmos()
        {
            if (_occupiedPositions == null)
            {
                return;
            }

            Gizmos.color = Color.black;
            for (int i = 0; i < _occupiedPositions.Length; i++)
            {
                Vector3 center = new(_occupiedPositions[i].x, 0, _occupiedPositions[i].y);
                Vector3 size = new(1, 0.1f, 1);
                Gizmos.DrawWireCube(ToWorldCoordinate(center), size);
            }

            Gizmos.color = Color.rebeccaPurple;
            Gizmos.DrawWireSphere(ToWorldCoordinate(new Vector3(lowerLeftCorner.x, 0, lowerLeftCorner.y)), 0.1f);
        }

        [Button("Fix Alignment")]
        private void FixAlignment()
        {
            transform.position = new Vector3(
                Mathf.RoundToInt(transform.position.x),
                0,
                Mathf.RoundToInt(transform.position.z)
            );
        }

        private Vector3 ToWorldCoordinate(Vector3 pos)
        {
            return pos + transform.position;
        }
    }
}