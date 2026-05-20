using Frostline.Core;
using UnityEngine;

namespace Frostline.World.Structures
{
    public class Structure : IPlaceable
    {
        public StructureBlueprint Blueprint { get; }
        private readonly Vector2Int[] _occupiedPositions;
        private readonly Vector2Int[] _bounds;
        private readonly Vector2Int _worldPosition;
        private readonly Rotation _rotation;
        public Vector2Int WorldPosition => _worldPosition;
        public Rotation Rotation => _rotation;

        public Structure(StructureBlueprint blueprint, Vector2Int worldPosition)
        {
            Blueprint = blueprint;
            _worldPosition = worldPosition;
            _occupiedPositions = blueprint.ToWorldPositions(worldPosition);
            _bounds = blueprint.BoundsToWorldPositions(worldPosition);
            _rotation = Rotation.Up;
        }
        public Structure(StructureBlueprint blueprint, Vector2Int worldPosition, Rotation rotation)
        {
            Blueprint = blueprint;
            _worldPosition = worldPosition;
            _occupiedPositions = blueprint.ToWorldPositions(worldPosition, rotation);
            _bounds = Blueprint.BoundsToWorldPositions(worldPosition, rotation);
            _rotation = rotation;
        }

        public Vector2Int[] GetOccupiedPositions()
        {
            return _occupiedPositions;
        }

        public Vector2Int[] Bounds()
        {
            return _bounds;
        }
    }
}