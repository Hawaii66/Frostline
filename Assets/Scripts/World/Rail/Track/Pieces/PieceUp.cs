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
        if (info.ShouldTendUp)
        {
            return 8;
        }

        return 1;
    }
}