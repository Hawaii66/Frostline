using System.Collections.Generic;
using UnityEngine;

namespace Frostline.Core
{
    class OccupiedMap<T> where T : IPlaceable
    {
        private readonly Dictionary<Vector2Int, T> _cells;

        public OccupiedMap()
        {
            _cells = new();
        }

        public bool CanPlace(T t)
        {
            Vector2Int[] cellsToTest = t.GetOccupiedPositions();
            for (int i = 0; i < cellsToTest.Length; i++)
            {
                if (IsOccupied(cellsToTest[i]))
                {
                    return false;
                }
            }

            return true;
        }
        public bool IsOccupied(Vector2Int pos)
        {
            return _cells.ContainsKey(pos);
        }
        public void Add(T t)
        {
            Vector2Int[] cellsToOccupy = t.GetOccupiedPositions();
            for (int i = 0; i < cellsToOccupy.Length; i++)
            {
                _cells.Add(cellsToOccupy[i], t);
            }
        }
        public void Remove(T t)
        {
            Vector2Int[] cellsToOccupy = t.GetOccupiedPositions();
            for (int i = 0; i < cellsToOccupy.Length; i++)
            {
                _cells.Remove(cellsToOccupy[i]);
            }
        }
        public bool TryGet(Vector2Int pos, out T t)
        {
            if (_cells.TryGetValue(pos, out t))
            {
                return true;
            }

            return false;
        }
    }
}