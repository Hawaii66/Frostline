using System.Collections.Generic;
using UnityEngine;

struct TrackResult
{
    public List<Vector2Int> path;
    public bool success;
}

class TrackGenerator
{
    TrackGeneration _track;
    public TrackGenerator()
    {
        _track = new TrackGeneration()
            .WithPiece(new PieceRight())
            .WithPiece(new PieceUp())
            .WithPiece(new PieceDiagonal())
            .WithPiece(new PieceUpU())
            .WithPiece(new PieceRightU())
            .WithPiece(new PieceWeird());
    }

    public TrackResult Generate(Vector2Int start, Vector2Int end)
    {
        int dirX = Dir(start.x, end.x);
        int dirY = Dir(start.y, end.y);
        Vector2Int localEnd = new(Mathf.Abs(end.x - start.x), Mathf.Abs(end.y - start.y));

        TrackResult result = _track.Generate(Vector2Int.zero, localEnd);
        if (!result.success)
        {
            return result;
        }

        for (int i = 0; i < result.path.Count; i++)
        {
            result.path[i] = new Vector2Int(
                ToWorldCoord(start.x, result.path[i].x, dirX),
                ToWorldCoord(start.y, result.path[i].y, dirY));
        }

        return result;
    }

    int Dir(int start, int end)
    {
        return end >= start ? 1 : -1;
    }
    int ToWorldCoord(int start, int local, int dir)
    {
        return start + local * dir;
    }
}