using UnityEngine;

class PieceRight : Piece
{
    public PieceRight()
    {
        segments = new()
        {
            new Segment
            {
                direction = Vector2Int.right,
                distancePercent = 0.05f,
                distancePercentVariation = 0.02f
            }
        };
    }

    public override int Weight(Vector2Int current, Vector2Int end, TrackInfo info)
    {
        if (info.ShouldTendRight)
        {
            return 8;
        }

        return 1;
    }
}