using System.Collections.Generic;
using UnityEngine;

public struct TrackInfo
{
    public bool ShouldTendUp;
    public bool ShouldTendRight;
    public bool HasReachedUp;
    public bool HasReachedRight;
}
public class Segment
{
    public Vector2Int direction;
    public float distancePercent;
    public float distancePercentVariation;
}
public abstract class Piece
{
    public List<Segment> segments;
    public abstract int Weight(Vector2Int current, Vector2Int end, TrackInfo info);
}