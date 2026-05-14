using UnityEngine;

namespace Frostline.Core
{
    public enum Rotation
    {
        Up,
        Down,
        Left,
        Right
    }
    public static class RotationManager
    {
        public static Vector2Int Rotate(Vector2Int a, Rotation rot)
        {
            return rot switch
            {
                Rotation.Up => new Vector2Int(a.x, a.y),
                Rotation.Right => new Vector2Int(a.y, -a.x),
                Rotation.Down => new Vector2Int(-a.x, -a.y),
                Rotation.Left => new Vector2Int(-a.y, a.x),
                _ => a,
            };
        }

        public static Vector2Int[] Rotate(Vector2Int[] a, Rotation rot)
        {
            Vector2Int[] rotated = new Vector2Int[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                rotated[i] = Rotate(a[i], rot);
            }
            return rotated;
        }
    }
}