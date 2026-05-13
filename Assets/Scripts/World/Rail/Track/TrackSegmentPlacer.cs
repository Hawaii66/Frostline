using Frostline.Core;
using Frostline.World.Structures;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Frostline.World.Tracks
{
    public class TrackSegmentManager : IRequireServices
    {
        public StructureBlueprintTrack[] TrackSegments;

        public void Initialize(IServiceRegistry serviceRegistry)
        {
            LoadTrackSegments();
        }

        private void LoadTrackSegments()
        {
            List<StructureBlueprintTrack> t = new();

            string[] guids = AssetDatabase.FindAssets($"t:{typeof(StructureBlueprintTrack).Name}", new[] { "Assets/Frostline/Structures" });
            StructureBlueprintTrack[] filtered = guids.Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<StructureBlueprintTrack>)
                .ToArray();

            for (int i = 0; i < filtered.Length; i++)
            {
                t.Add(filtered[i]);
            }

            TrackSegments = t.ToArray();
        }
    }

    public class TrackSegmentPlacer : IRequireServices
    {
        private TrackSegmentManager _trackSegmentManager;

        public void Initialize(IServiceRegistry serviceRegistry)
        {
            _trackSegmentManager = serviceRegistry.GetService<TrackSegmentManager>();

            List<Vector2Int> testPath = new()
            {
                new(0,5),
                new(1,5),
                new(2,5),
                new(3,6),
                new(4,7),
                new(5,8),
                new(6,9),
            };

            string[] result = GeneratePath(testPath);
            Debug.Log("Path generation result");
            if (result == null)
            {
                Debug.Log("Path not found");
            }
            else
            {
                for (int i = 0; i < result.Length; i++)
                {
                    Debug.Log(result[i]);
                }
            }
        }

        public string[] GeneratePath(List<Vector2Int> path)
        {
            int n = path.Count;
            bool[] reachable = new bool[n];
            int[] parentIndex = new int[n];
            string[] parentSegment = new string[n];

            for (int i = 0; i < n; i++)
            {
                parentIndex[i] = -1;
            }

            reachable[0] = true;

            for (int i = 0; i < n; ++i)
            {
                if (!reachable[i])
                {
                    continue;
                }

                for (int j = 0; j < _trackSegmentManager.TrackSegments.Length; ++j)
                {
                    StructureBlueprintTrack sbt = _trackSegmentManager.TrackSegments[j];
                    if (!Matches(path, i, sbt.TrackSegments))
                    {
                        continue;
                    }

                    int next = i + sbt.TrackSegments.Length - 1;
                    if (next >= n || next == i)
                    {
                        continue;
                    }

                    if (!reachable[next])
                    {
                        reachable[next] = true;
                        parentIndex[next] = i;
                        parentSegment[next] = sbt.name;
                    }
                }
            }

            if (!reachable[n - 1])
            {
                return null;
            }

            List<string> result = new();
            int current = n - 1;

            while (current != 0)
            {
                result.Add(parentSegment[current]);
                current = parentIndex[current];
            }

            result.Reverse();
            return result.ToArray();
        }

        private bool Matches(List<Vector2Int> path, int startIndex, Vector2Int[] segment)
        {
            int endIndex = startIndex + segment.Length - 1;
            if (endIndex >= path.Count)
            {
                return false;
            }

            Vector2Int start = path[startIndex];

            for (int i = 0; i < segment.Length; i++)
            {
                Vector2Int expected = start + segment[i];
                if (path[startIndex + i] != expected)
                {
                    return false;
                }
            }

            return true;
        }
    }
}