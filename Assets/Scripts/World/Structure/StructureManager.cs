using Frostline.Core;
using Frostline.World.Tiles;
using System.Collections.Generic;
using UnityEngine;

namespace Frostline.World.Structures
{
    public class StructureManager : IRequireServices
    {
        private readonly List<Structure> _structures;
        public IReadOnlyList<Structure> Structures => _structures;
        private readonly OccupiedMap<IPlaceable> _occupiedCells;
        private TileManager _tileManager;

        public StructureManager()
        {
            _structures = new();
            _occupiedCells = new();
        }

        public bool TryAddStructure(Structure structure)
        {
            if (!_occupiedCells.CanPlace(structure))
            {
                return false;
            }
            if (!CanPlaceAtHeight(structure))
            {
                return false;
            }

            _occupiedCells.Add(structure);
            _structures.Add(structure);
            return true;
        }

        private bool CanPlaceAtHeight(Structure structure)
        {
            Vector2Int[] occupiedPositions = structure.GetOccupiedPositions();
            int height = _tileManager.GetTile(occupiedPositions[0]).HeightInt;
            for (int i = 1; i < occupiedPositions.Length; i++)
            {
                int height2 = _tileManager.GetTile(occupiedPositions[i]).HeightInt;
                if (height2 != height)
                {
                    return false;
                }
            }

            return true;
        }

        public void Initialize(IServiceRegistry serviceRegistry)
        {
            _tileManager = serviceRegistry.GetService<TileManager>();
        }
    }
}