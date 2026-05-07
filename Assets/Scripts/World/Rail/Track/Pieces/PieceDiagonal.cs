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
        float balance = (float)Mathf.Min(info.DiffX, info.DiffY) /
                    Mathf.Max(info.DiffX, info.DiffY);

        if (balance > 0.95f)
        {
            return 8;
        }
        if (balance > 0.85f)
        {
            return 6;
        }

        return 2;
    }

    public override bool IsMinimumDistancePossible()
    {
        return true;
    }
}