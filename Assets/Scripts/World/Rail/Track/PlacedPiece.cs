
using System.Collections.Generic;
using UnityEngine;

class PlacedPiece
{
    public List<Vector2Int> path;
    public PlacedPiece() { }
    public bool Generate(Vector2Int start, Piece piece, int size, bool minimumDistanceRequired, System.Random random)
    {
        path = new();
        Vector2Int pos = start;
        for (int i = 0; i < piece.segments.Count; i++)
        {
            Segment segment = piece.segments[i];
            float t = (float)random.NextDouble();
            float distance = segment.distancePercent + (t * 2 - 1) * segment.distancePercentVariation;
            Vector2Int goal = pos + segment.direction * (minimumDistanceRequired ? 1 : Mathf.RoundToInt(distance * size));
            int safe = 0;
            while (pos != goal)
            {
                safe += 1;
                if (safe > 10000)
                {
                    return false;
                }

                pos += segment.direction;
                path.Add(pos);
            }
        }

        if (path.Count == 0)
        {
            return false;
        }

        return true;
    }
}
