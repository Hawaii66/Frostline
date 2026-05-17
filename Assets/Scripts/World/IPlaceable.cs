using UnityEngine;

namespace Frostline.Core
{
    public interface IPlaceable
    {
        public Vector2Int[] GetOccupiedPositions();
    }
}