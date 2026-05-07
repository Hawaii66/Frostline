
using System.Collections.Generic;
using UnityEngine;


class PlacedPiece
{
    public List<Vector2Int> path;
    public PlacedPiece() { }
    public bool Generate(Vector2Int start, Piece piece, int size, bool minimumDistanceRequired)
    {
        path = new();
        Vector2Int pos = start;
        for (int i = 0; i < piece.segments.Count; i++)
        {
            Segment segment = piece.segments[i];
            float distance = segment.distancePercent + Random.Range(-segment.distancePercentVariation, segment.distancePercentVariation);
            Vector2Int goal = pos + segment.direction * (minimumDistanceRequired ? 1 : Mathf.RoundToInt(distance * size));
            int safe = 0;
            while (pos != goal)
            {
                safe += 1;
                if(safe > 10000)
                {
                    return false;
                }

                pos += segment.direction;
                path.Add(pos);
            }
        }

        return true;
    }
}

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

        return 1;
    }
}
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
                distancePercentVariation = 0.02f
            },
            new Segment
            {
                direction = Vector2Int.up,
                distancePercent = 0.05f,
                distancePercentVariation = 0.02f
            },
            new Segment
            {
                direction = Vector2Int.left,
                distancePercent = 0.05f,
                distancePercentVariation = 0.02f
            },
            new Segment
            {
                direction = Vector2Int.up,
                distancePercent = 0.05f,
                distancePercentVariation = 0.02f
            },
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

class Track
{
    static List<Piece> pieces = new() { 
        new PieceRight(),
        new PieceUp(),
        new PieceDiagonal(),
        new PieceWeird()
       };

    public static bool Fits(List<Vector2Int> visited, PlacedPiece piece, Vector2Int start,  Vector2Int end)
    {
        for(int i = 0; i < piece.path.Count; i++)
        {
            Vector2Int pos = piece.path[i];
            if(pos.x < start.x || pos.y < start.y ||pos.x > end.x || pos.y > end.y)
            {
                return false;
            }
            if (visited.Contains(pos))
            {
                return false;
            }
        }

        return true;
    }

    public static List<Vector2Int> Generate(Vector2Int start, Vector2Int end)
    {
        int sizeX = end.x - start.x;
        int sizeY = end.y - start.y;
        int size = sizeX < sizeY ? sizeX : sizeY;
       
        List<Vector2Int> visited = new()
        {
            start
        };

        Vector2Int position = start;

        int maxIterations = 10000;
        int safe = 0;
        bool minimumDistanceRequired = false;
        while(safe ++ < maxIterations)
        {
            if(position == end)
            {
                Debug.Log("End reached");
                return visited;
            }

            int diffX = end.x - position.x;
            int diffY = end.y - position.y;

            List<PlacedPiece> piecesToTest = new();
            for(int i = 0; i < pieces.Count; i++)
            {
                Piece piece = pieces[i];
                PlacedPiece placedPiece = new PlacedPiece(position, piece, size, minimumDistanceRequired);
                if(Fits(visited, placedPiece, start, end))
                {
                    int weight = piece.Weight(position, end, new TrackInfo
                    {
                        HasReachedRight = position.x == end.x,
                        HasReachedUp = position.y == end.y,
                        ShouldTendRight = diffX > diffY,
                        ShouldTendUp = diffY > diffX,
                    });
                    for(int j = 0; j < weight; j++)
                    {
                        piecesToTest.Add(placedPiece);
                    }
                }
            }

            if(piecesToTest.Count == 0)
            {
                minimumDistanceRequired = true;
                continue;
            }
            minimumDistanceRequired = false;

            PlacedPiece chosenPlacedPiece = piecesToTest[Random.Range(0, piecesToTest.Count)];
            for(int i = 0; i < chosenPlacedPiece.path.Count; i++)
            {
                Vector2Int move = chosenPlacedPiece.path[i];
                position = move;
                visited.Add(position);
            }
        }

        Debug.Log("Fail safe");
        return visited;
    }
}