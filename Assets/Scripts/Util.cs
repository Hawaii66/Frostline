
using UnityEngine;

namespace Frostline.Core
{
    public static class Util
    {
        public static Vector2Int[] CardinalOffsets = new Vector2Int[]
        {
            new(1,0),
            new (-1,0),
            new (0,1),
            new (0,-1),
        };
        public static Vector2Int[] DiagonalOffsets = new Vector2Int[]
        {
            new(1,1),
            new(1,-1),
            new(-1,1),
            new(-1,-1),
        };

        public static Color FromGrayScale(float t)
        {
            return new Color(t, t, t);
        }
        public static Color FromGrayScaleRange(float min, float max, float t)
        {
            return FromGrayScale((t - min) / (max - min));
        }

        public static float SmoothStep(float e0, float e1, float x)
        {
            float t = Mathf.Max(0, Mathf.Min(1, (x - e0) / (e1 - e0)));
            return t * t * (3 - 2 * t);
        }
        public static int Sign(int x)
        {
            if (x < 0)
            {
                return -1;
            }
            if (x > 0)
            {
                return 1;
            }
            return 0;
        }
    }
}