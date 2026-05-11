using UnityEngine;

namespace Frostline.Core
{
    interface IPlaceable
    {
        public Vector2Int[] GetOccupiedPositions();
    }
}