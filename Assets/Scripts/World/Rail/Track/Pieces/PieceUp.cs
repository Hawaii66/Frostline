using UnityEngine;

class PieceUp : Piece
{
    public PieceUp()
    {
        segments = new()
        {
            new Segment
            {
                direction = Vector2Int.up,
                distancePercent = 0.05f,
                distancePercentVariation = 0.02f
            }
        };
    }

    public override int Weight(Vector2Int current, Vector2Int end, TrackInfo info)
    {
        float total = info.DiffX + info.DiffY;
        float yShare = info.DiffY / total;
        if (yShare > 0.98f)
        {
            return 48;
        }
        if (yShare > 0.95f)
        {
            return 24;
        }
        if (yShare > 0.85f)
        {
            return 12;
        }
        if (yShare > 0.65f)
        {
            return 8;
        }

        return 1;
    }

    public override bool IsMinimumDistancePossible()
    {
        return true;
    }
}