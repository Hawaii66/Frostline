using System.Collections.Generic;
using UnityEngine;

namespace Frostline.Core
{
    public class OccupiedMap<T> where T : IPlaceable
    {
        private readonly Dictionary<Vector2Int, T> _cells;

        public OccupiedMap()
        {
            _cells = new();
        }
        public bool CanPlace(T t)
        {
            return CanPlace(t.GetOccupiedPositions());
        }
        public bool CanPlace(Vector2Int[] offsets)
        {
            for (int i = 0; i < offsets.Length; i++)
            {
                if (IsOccupied(offsets[i]))
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
            Add(t, t.GetOccupiedPositions());
        }
        public void Add(T t, Vector2Int[] offsets)
        {
            for (int i = 0; i < offsets.Length; i++)
            {
                _cells.Add(offsets[i], t);
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
        public T[] AsArray()
        {
            T[] values = new T[_cells.Count];

            int i = 0;
            foreach (T t in _cells.Values)
            {
                values[i] = t;
                i++;
            }
            return values;
        }
    }
}