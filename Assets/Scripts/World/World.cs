using Frostline.Core;
using Frostline.World.Structures;
using System.Collections.Generic;

namespace Frostline.World
{
    public class WorldContext
    {
        private readonly List<Structure> _structures;
        public IReadOnlyList<Structure> Structures => _structures;
        private readonly OccupiedMap<IPlaceable> _occupiedCells;

        public WorldContext()
        {
            _structures = new();
            _occupiedCells = new();
        }

        public bool TryAddStructure(Structure structure)
        {
            if (_occupiedCells.CanPlace(structure))
            {
                _occupiedCells.Add(structure);
                _structures.Add(structure);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}