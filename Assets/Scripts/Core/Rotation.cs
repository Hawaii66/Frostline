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

        public static Rotation CardinalOffsetToDirection(Vector2Int offset)
        {
            if (offset.x == 1)
            {
                return Rotation.Right;
            }
            if (offset.x == -1)
            {
                return Rotation.Left;
            }
            if (offset.y == 1)
            {
                return Rotation.Up;
            }
            if (offset.y == -1)
            {
                return Rotation.Down;
            }
            return Rotation.Up;
        }
        public static Rotation FlipRotation(Rotation rot)
        {
            if (rot == Rotation.Right)
            {
                return Rotation.Left;
            }
            if (rot == Rotation.Left)
            {
                return Rotation.Right;
            }
            if (rot == Rotation.Up)
            {
                return Rotation.Down;
            }
            if (rot == Rotation.Down)
            {
                return Rotation.Up;
            }
            return Rotation.Up;
        }
    }
}