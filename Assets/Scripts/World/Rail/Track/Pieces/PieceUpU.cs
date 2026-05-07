using UnityEngine;

class PieceUpU : Piece
{
    public PieceUpU()
    {
        segments = new()
        {
            new Segment
            {
                direction = Vector2Int.up,
                distancePercent = 0.05f,
                distancePercentVariation = 0.02f
            },
            new Segment
            {
                direction = Vector2Int.right,
                distancePercent = 0.05f,
                distancePercentVariation = 0.02f
            },
            new Segment
            {
                direction = Vector2Int.down,
                distancePercent = 0.05f,
                distancePercentVariation = 0.02f
            },
            new Segment
            {
                direction = Vector2Int.right,
                distancePercent = 0.05f,
                distancePercentVariation = 0.02f
            },
        };
    }

    public override int Weight(Vector2Int current, Vector2Int end, TrackInfo info)
    {
        return 3;
    }
}