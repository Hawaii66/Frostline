using Frostline.World.Structures.Editor;
using NaughtyAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Frostline.World.Structures
{
    public class StructureBlueprintMetadata : MonoBehaviour
    {
        [SerializeField] private Vector2Int[] _occupiedPositions;
        [SerializeField] private Vector2Int _boundCenterOffset;
        [SerializeField] private Vector2Int _boundSize;
        public string Name;

        [Button("Generate Blueprint")]
        private void GenerateBlueprint()
        {
            StructureBlueprintGenerator.GenerateStructure(transform);
        }

        public Vector2Int[] GetOccupiedTiles()
        {
            return _occupiedPositions;
        }
        public Vector2Int[] GetBounds()
        {
            List<Vector2Int> bounds = new();
            for (int x = -_boundSize.x; x < _boundSize.x + 1; x++)
            {
                for (int y = -_boundSize.y; y < _boundSize.y + 1; y++)
                {
                    bounds.Add(new(x + _boundCenterOffset.x, y + _boundCenterOffset.y));
                }
            }
            return bounds.ToArray();
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
            Gizmos.DrawWireSphere(ToWorldCoordinate(new Vector3(0, 0, 0)), 0.1f);

            Gizmos.color = Color.coral;
            Vector3 boundCenterOffset = new(_boundCenterOffset.x, 0, _boundCenterOffset.y);
            Vector3 boundSize = new(_boundSize.x * 2, 0.1f, _boundSize.y * 2);
            Gizmos.DrawWireCube(ToWorldCoordinate(boundCenterOffset), boundSize);
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

        [Button("Rotate Occupied Positions")]
        private void RotateOccupiedPositions()
        {
            for (int i = 0; i < _occupiedPositions.Length; i++)
            {
                _occupiedPositions[i] = new(_occupiedPositions[i].y, -_occupiedPositions[i].x);
            }
        }
        [Button("Flip X")]
        private void FlipX()
        {
            for (int i = 0; i < _occupiedPositions.Length; i++)
            {
                _occupiedPositions[i] = new(-_occupiedPositions[i].x, _occupiedPositions[i].y);
            }
        }
        [Button("Flip Y")]
        private void FlipY()
        {
            for (int i = 0; i < _occupiedPositions.Length; i++)
            {
                _occupiedPositions[i] = new(_occupiedPositions[i].x, -_occupiedPositions[i].y);
            }
        }

        [SerializeField] private Vector2Int _moveOccupiedPositons;
        [Button("Move")]
        private void Move()
        {
            for (int i = 0; i < _occupiedPositions.Length; i++)
            {
                _occupiedPositions[i] += _moveOccupiedPositons;
            }
            _moveOccupiedPositons = new();
        }
    }
}