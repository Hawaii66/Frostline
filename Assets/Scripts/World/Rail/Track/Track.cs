
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

class Track
{
    static Vector2Int Right = new (1,0);
    static Vector2Int Up = new (0, 1);
    static Vector2Int Diagonal = new (1,1);
    public static List<Vector2Int> Generate(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> visited = new()
        {
            start
        };

        Vector2Int position = start;
        while (true)
        {
            List<Vector2Int> possibleMoves = new List<Vector2Int>();
            if(position.x != end.x)
            {
                possibleMoves.Add(Right);
            }
            if(position.y != end.y)
            {
                possibleMoves.Add(Up);
            }
            if(position.x != end.x && position.y != end.y)
            {
                possibleMoves.Add(Diagonal);
            }
            int diffX = end.x - position.x;
            int diffY = end.y - position.y;
            if(diffX > diffY)
            {
                possibleMoves.Add(Right);
                possibleMoves.Add(Right);
                possibleMoves.Add(Right);
                possibleMoves.Add(Right);
                possibleMoves.Add(Right);
                possibleMoves.Add(Right);
            }
            else if(diffY > diffX)
            {
                possibleMoves.Add(Up);
                possibleMoves.Add(Up);
                possibleMoves.Add(Up);
                possibleMoves.Add(Up);
                possibleMoves.Add(Up);
                possibleMoves.Add(Up);
            }
            else
            {
                possibleMoves.Add(Diagonal);
                possibleMoves.Add(Diagonal);
                possibleMoves.Add(Diagonal);
                possibleMoves.Add(Diagonal);
                possibleMoves.Add(Diagonal);
                possibleMoves.Add(Diagonal);
            }

            int nextOptionIndex = Random.Range(0, possibleMoves.Count);
            Vector2Int nextOption = possibleMoves[nextOptionIndex];

            Vector2Int nextPos = position + nextOption;
            if (nextPos.x > end.x || nextPos.y > end.y || nextPos.x < start.x || nextPos.y < start.y)
            {
                continue;
            }
            
            visited.Add(nextPos);
            if(nextPos.x == end.x && nextPos.y == end.y)
            {
                return visited;
            }

            position = nextPos;
        }
    } 
}