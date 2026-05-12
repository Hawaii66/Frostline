
using System.Collections.Generic;
using UnityEngine;

class TrackGeneration
{
    List<Piece> _pieces;

    public TrackGeneration()
    {
        _pieces = new();
    }

    public TrackGeneration WithPiece(Piece piece)
    {
        _pieces.Add(piece);
        return this;
    }

    public bool PlacedPieceFits(HashSet<Vector2Int> visited, PlacedPiece piece, Vector2Int start, Vector2Int end)
    {
        for (int i = 0; i < piece.path.Count; i++)
        {
            Vector2Int pos = piece.path[i];
            if (pos.x < start.x || pos.y < start.y || pos.x > end.x || pos.y > end.y)
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

    public TrackResult Generate(Vector2Int start, Vector2Int end)
    {
        int sizeX = end.x - start.x;
        int sizeY = end.y - start.y;
        int size = sizeX < sizeY ? sizeX : sizeY;

        List<Vector2Int> visited = new()
        {
            start
        };
        HashSet<Vector2Int> visitedSet = new() { start };

        Vector2Int position = start;

        int safe = 0;
        int minimumDistanceRequired = 0;
        while (safe++ < 10000)
        {
            if (position == end)
            {
                return new TrackResult
                {
                    path = visited,
                    success = true
                };
            }

            int diffX = end.x - position.x;
            int diffY = end.y - position.y;

            List<PlacedPiece> piecesToTest = new();
            for (int i = 0; i < _pieces.Count; i++)
            {
                Piece piece = _pieces[i];
                if (minimumDistanceRequired != 0 && !piece.IsMinimumDistancePossible())
                {
                    continue;
                }

                PlacedPiece placedPiece = new();
                bool canGenerate = placedPiece.Generate(position, piece, size, minimumDistanceRequired != 0);
                if (!canGenerate)
                {
                    continue;
                }
                if (PlacedPieceFits(visitedSet, placedPiece, start, end))
                {
                    int weight = piece.Weight(position, end, new TrackInfo
                    {
                        HasReachedRight = position.x == end.x,
                        HasReachedUp = position.y == end.y,
                        DiffX = diffX,
                        DiffY = diffY
                    });
                    for (int j = 0; j < weight; j++)
                    {
                        piecesToTest.Add(placedPiece);
                    }
                }
            }

            if (piecesToTest.Count == 0)
            {
                minimumDistanceRequired = 10;
                continue;
            }
            minimumDistanceRequired = -1;

            PlacedPiece chosenPlacedPiece = piecesToTest[Random.Range(0, piecesToTest.Count)];
            for (int i = 0; i < chosenPlacedPiece.path.Count; i++)
            {
                Vector2Int move = chosenPlacedPiece.path[i];
                position = move;
                visited.Add(position);
                visitedSet.Add(position);
            }
        }

        return new TrackResult
        {
            success = false
        };
    }
}