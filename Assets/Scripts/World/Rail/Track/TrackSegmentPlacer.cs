using Frostline.Core;
using Frostline.World.Generation;
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
        public Dictionary<string, StructureBlueprintTrack> TrackSegmentDict;
        private Dictionary<Edge, string[]> _generatedTrackSegments;
        public List<List<Vector2Int>> TrackPaths;

        public TrackSegmentManager()
        {
            _generatedTrackSegments = new();
        }

        public bool HasTrack(Vector2Int start, Vector2Int end)
        {
            foreach (Edge edge in _generatedTrackSegments.Keys)
            {
                if (edge.A.Polar.ToCartesianInt() == start && edge.B.Polar.ToCartesianInt() == end)
                {
                    return true;
                }
            }
            return false;
        }

        public void Initialize(IServiceRegistry serviceRegistry)
        {
            LoadTrackSegments();
        }

        private void LoadTrackSegments()
        {
            List<StructureBlueprintTrack> t = new();
            TrackSegmentDict = new();

            string[] guids = AssetDatabase.FindAssets($"t:{typeof(StructureBlueprintTrack).Name}", new[] { "Assets/Frostline/Structures" });
            StructureBlueprintTrack[] filtered = guids.Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<StructureBlueprintTrack>)
                .ToArray();

            for (int i = 0; i < filtered.Length; i++)
            {
                t.Add(filtered[i]);
                TrackSegmentDict.Add(filtered[i].StructureBlueprint.Name, filtered[i]);
            }

            TrackSegments = t.ToArray();
        }

        public void GenerateTrackSegments(Edge edge, List<Vector2Int> path)
        {
            string[] segments = PathToSegments(path);
            if (segments == null)
            {
                Debug.Log($"Failed to generate path ${edge.A.Polar.ToCartesian()} {edge.B.Polar.ToCartesian()} {path.Count}");
                return;
            }
            else
            {
                Debug.Log($"Success ${edge.A.Polar.ToCartesian()} {edge.B.Polar.ToCartesian()} {path.Count}");

            }

            _generatedTrackSegments.Add(edge, segments);
        }

        private string[] PathToSegments(List<Vector2Int> path)
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

                for (int j = 0; j < TrackSegments.Length; ++j)
                {
                    StructureBlueprintTrack sbt = TrackSegments[j];
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
                        parentSegment[next] = sbt.StructureBlueprint.Name;
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