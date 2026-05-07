using UnityEngine;

class PieceWeird : Piece
{
    public PieceWeird()
    {
        segments = new()
        {
            new Segment
            {
                direction = Vector2Int.right,
                distancePercent = 0.05f,
            },
            new Segment
            {
                direction = Vector2Int.up,
                distancePercent = 0.05f,
            },
            new Segment
            {
                direction = new Vector2Int(-1, -1),
                distancePercent = 0.025f,
            },
            new Segment
            {
                direction = new Vector2Int(-1, 1),
                distancePercent = 0.025f,
            },
            new Segment
            {
                direction = Vector2Int.up,
                distancePercent = 0.05f,
            },
        };
    }

    public override int Weight(Vector2Int current, Vector2Int end, TrackInfo info)
    {
        return 2;
    }
}