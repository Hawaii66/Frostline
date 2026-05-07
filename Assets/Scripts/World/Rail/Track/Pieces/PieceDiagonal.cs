using UnityEngine;

class PieceDiagonal : Piece
{
    public PieceDiagonal()
    {
        segments = new()
        {
            new Segment
            {
                direction = new Vector2Int(1,1),
                distancePercent = 0.05f,
                distancePercentVariation = 0.02f
            },
        };
    }

    public override int Weight(Vector2Int current, Vector2Int end, TrackInfo info)
    {
        if (!info.ShouldTendUp && !info.ShouldTendRight)
        {
            return 4;
        }

        return 2;
    }
}