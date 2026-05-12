
using Frostline.World.Heights;
using JetBrains.Annotations;
using Mono.Cecil;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Frostline.World.Tracks
{
    public class TrackNetwork : IHeightScale
    {
        private List<List<Vector2Int>> _trackPaths;
        private readonly int _sizeX;
        private readonly int _sizeY;
        public int[,] DistanceToTrack { get; private set; }
        public int MaxDistanceToTrack { get; private set; }

        private readonly static Vector2Int[] _offsets10 = new Vector2Int[]
        {
            new(1,0),
            new(0,1),
            new(-1,0),
            new(0,-1),
        };
        private readonly static Vector2Int[] _offsets14 = new Vector2Int[]
        {
            new(-1,-1),
            new(1,-1),
            new(-1,1),
            new(1,1),
        };

        public TrackNetwork(int sizeX, int sizeY)
        {
            _sizeX = sizeX;
            _sizeY = sizeY;
            _trackPaths = new();
            DistanceToTrack = new int[sizeX, sizeY];
        }

        public void SetTrackPaths(List<List<Vector2Int>> trackPaths)
        {
            _trackPaths = trackPaths;
            CalculateDistanceToTrack();
        }

        private void CalculateDistanceToTrack()
        {
            DistanceToTrack = new int[_sizeX, _sizeY];
            for (int x = 0; x < _sizeX; x++)
            {
                for (int y = 0; y < _sizeY; y++)
                {
                    DistanceToTrack[x, y] = -1;
                }
            }

            Queue<Vector2Int> toProcess = new();
            HashSet<Vector2Int> visited = new();

            for (int i = 0; i < _trackPaths.Count; i++)
            {
                List<Vector2Int> trackPath = _trackPaths[i];
                for (int j = 0; j < trackPath.Count; j++)
                {
                    Vector2Int p = trackPath[j];
                    if (visited.Add(p))
                    {
                        DistanceToTrack[p.x, p.y] = 0;
                        toProcess.Enqueue(p);
                    }
                }
            }

            while (toProcess.Count > 0)
            {
                Vector2Int position = toProcess.Dequeue();

                int distance = DistanceToTrack[position.x, position.y];

                for (int i = 0; i < _offsets10.Length; i++)
                {
                    Vector2Int offstPosition = position + _offsets10[i];
                    if (!IsInWorld(offstPosition))
                    {
                        continue;
                    }
                    if (!visited.Add(offstPosition))
                    {
                        continue;
                    }

                    DistanceToTrack[offstPosition.x, offstPosition.y] = distance + 10;
                    MaxDistanceToTrack = Mathf.Max(MaxDistanceToTrack, distance + 10);
                    toProcess.Enqueue(offstPosition);
                }

                for (int i = 0; i < _offsets14.Length; i++)
                {
                    Vector2Int offstPosition = position + _offsets14[i];
                    if (!IsInWorld(offstPosition))
                    {
                        continue;
                    }
                    if (!visited.Add(offstPosition))
                    {
                        continue;
                    }

                    DistanceToTrack[offstPosition.x, offstPosition.y] = distance + 14;
                    MaxDistanceToTrack = Mathf.Max(MaxDistanceToTrack, distance + 14);
                    toProcess.Enqueue(offstPosition);
                }
            }
        }

        private bool IsInWorld(Vector2Int position)
        {
            if (position.x < 0 || position.y < 0)
            {
                return false;
            }
            if (position.x >= _sizeX || position.y >= _sizeY)
            {
                return false;
            }
            return true;
        }

        public float Scale(int x, int y)
        {
            float blendDistance = MaxDistanceToTrack * 0.025f;
            int distance = DistanceToTrack[x, y];

            if (distance > blendDistance)
            {
                return 0;
            }

            return 1 - SmoothStep(0, blendDistance, distance);
        }

        private static float SmoothStep(float e0, float e1, float x)
        {
            float t = Mathf.Max(0, Mathf.Min(1, (x - e0) / (e1 - e0)));
            return t * t * (3 - 2 * t);
        }
    }
}