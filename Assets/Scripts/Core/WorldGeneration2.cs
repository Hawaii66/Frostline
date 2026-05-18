using Frostline.Core;
using Frostline.World;
using Frostline.World.Structures;
using Frostline.World.Tiles;
using Frostline.World.Tracks;
using System.Collections.Generic;
using UnityEngine;

namespace Frostline.Test
{

    public class TrackPathResult
    {
        public List<Vector2Int> Path;
        public List<string> Segments;
        public Vector2Int Start;
        public Vector2Int End;
    }

    public class WorldGenerationResult
    {
        public Tile[,] Tiles;
        public List<Structure> Structures;
        public List<TrackPathResult> TrackPaths;
        public List<Vector2Int> TrackNodes;
        public List<(Vector2Int, Vector2Int)> TrackEdges;
        public int[,] JunctionPredictor;
        public OccupiedMap<IPlaceable> OccupiedMap;
        public List<DebugText> DebugTexts;
    }
    public class DebugText
    {
        public string Text;
        public Vector2Int Position;
    }

    public class WorldGeneration2 : IRequireServices
    {
        private TrackSegmentManager _trackSegmentManager;
        private StructureBlueprintManager _structureBlueprintManager;
        private WorldSettings _worldSettings;
        private OccupiedMap<IPlaceable> _occupiedMap;

        public WorldGenerationResult Generate(Vector2Int size)
        {
            TileGeneration tileGeneration = new(_worldSettings.TileSettings, size.x, size.y);
            Tile[,] tiles = tileGeneration.GenerateTiles();
            List<Structure> structures = new();
            List<Vector2Int> trackNodes = new();
            List<(Vector2Int, Vector2Int)> trackEdges = new();
            List<Vector2Int[]> trackPointsList = new();
            List<BoundFiller> fillers = new();
            List<DebugText> debugTexts = new();

            _occupiedMap = new();
            _structureBlueprintManager.TryGetStructureBlueprint("TestT_SB", out StructureBlueprint cubeBlueprint);
            _structureBlueprintManager.TryGetStructureBlueprint("Center_SB", out StructureBlueprint centerBlueprint);
            _structureBlueprintManager.TryGetStructureBlueprint("TJunction_SB", out StructureBlueprint tJunctionBlueprint);
            _trackSegmentManager.TrackSegmentDict.TryGetValue("TestT", out StructureBlueprintTrack trackT);
            _trackSegmentManager.TrackSegmentDict.TryGetValue("Center", out StructureBlueprintTrack centerT);
            _trackSegmentManager.TrackSegmentDict.TryGetValue("TJunction", out StructureBlueprintTrack junctionT);

            Vector2Int center = size / 2;
            Structure centerStructure = new(centerBlueprint, center);
            if (!_occupiedMap.CanPlace(centerStructure.GetBounds()))
            {
                Debug.LogError($"Failed to place center structures");
                return null;
            }
            _occupiedMap.Add(centerStructure, centerStructure.GetBounds());
            structures.Add(centerStructure);
            ExpandStructureBounds(centerStructure.GetBounds(), size, _occupiedMap, fillers);
            ExpandTrackPath(center, centerT.TrackPaths, Rotation.Up, trackPointsList, trackNodes, trackEdges);

            int[,] junctionPredictor = new int[size.x, size.y];

            int structureCount = 20;
            for (int i = 0; i < structureCount; i++)
            {
                int maxAttempts = 20;
                for (int j = 0; j < maxAttempts; j++)
                {
                    int randX = Random.Range(10, size.x - 10);
                    int randY = Random.Range(10, size.y - 10);

                    int diffX = randX - center.x;
                    int diffY = randY - center.y;
                    int diffXAbs = Mathf.Abs(diffX);
                    int diffYAbs = Mathf.Abs(diffY);
                    Rotation rot;
                    //Rotations should swap when corrected for train track in structure blueprint
                    if (diffXAbs > diffYAbs)
                    {
                        if (diffX > 0)
                        {
                            rot = Rotation.Right;
                        }
                        else
                        {
                            rot = Rotation.Left;
                        }
                    }
                    else
                    {
                        if (diffY > 0)
                        {
                            rot = Rotation.Up;
                        }
                        else
                        {
                            rot = Rotation.Down;
                        }
                    }

                    Vector2Int gridPos = new(randX, randY);
                    Structure cubeStructure = new(cubeBlueprint, gridPos, rot);
                    if (_occupiedMap.CanPlace(cubeStructure.GetBounds()))
                    {
                        _occupiedMap.Add(cubeStructure, cubeStructure.GetBounds());
                        structures.Add(cubeStructure);
                        ExpandStructureBounds(cubeStructure.GetBounds(), size, _occupiedMap, fillers);

                        ExpandTrackPath(gridPos, trackT.TrackPaths, rot, trackPointsList, trackNodes, trackEdges);
                        break;
                    }
                }
            }

            foreach (Vector2Int[] trackPoints in trackPointsList)
            {
                Vector2Int diffTrackPointsLarge = trackPoints[trackPoints.Length - 1] - trackPoints[trackPoints.Length - 2];
                Vector2Int diffTrackPoints = new(Sign(diffTrackPointsLarge.x), Sign(diffTrackPointsLarge.y));

                Vector2Int pos = trackPoints[trackPoints.Length - 1];
                junctionPredictor[pos.x, pos.y] = -1;
                pos += diffTrackPoints;

                while (true)
                {
                    if (pos.x < 0 || pos.x > size.x - 1 || pos.y < 0 || pos.y > size.y - 1) { break; }
                    //if (_occupiedMap.IsOccupied(pos)) { break; }

                    junctionPredictor[pos.x, pos.y] += 1;
                    if (junctionPredictor[pos.x, pos.y] > 1) { break; }
                    pos += diffTrackPoints;
                }
            }

            foreach (BoundFiller filler in fillers)
            {
                _occupiedMap.Remove(filler);
            }
            fillers.Clear();

            GenerateExtraJunctions(junctionPredictor, size, _occupiedMap, debugTexts);
            return new WorldGenerationResult { DebugTexts = debugTexts, OccupiedMap = _occupiedMap, JunctionPredictor = junctionPredictor, Structures = structures, Tiles = tiles, TrackPaths = null, TrackEdges = trackEdges, TrackNodes = trackNodes };

            Dictionary<Vector2Int, Structure> nodeToJunction = new();
            for (int x = 0; x < junctionPredictor.GetLength(0); x++)
            {
                for (int y = 0; y < junctionPredictor.GetLength(1); y++)
                {
                    if (junctionPredictor[x, y] == 2)
                    {
                        Vector2Int node = new(x, y);
                        Rotation rot = JunctionRotation(node, junctionPredictor);
                        Vector2Int movedPosition = MoveJunction(node, rot, tJunctionBlueprint, _occupiedMap, size);
                        Structure str = AddJunctionAndEdges(node, movedPosition, size, tJunctionBlueprint, junctionT, structures, _occupiedMap, debugTexts, junctionPredictor, trackNodes, trackEdges, trackPointsList);
                        if (str != null)
                        {
                            nodeToJunction.Add(node, str);
                        }
                    }
                }
            }

            HashSet<Vector2Int> visited = new();
            for (int x = 0; x < junctionPredictor.GetLength(0); x++)
            {
                for (int y = 0; y < junctionPredictor.GetLength(1); y++)
                {
                    if (junctionPredictor[x, y] == 0)
                    {
                        continue;
                    }
                    if (junctionPredictor[x, y] == 2)
                    {
                        continue;
                    }
                    Vector2Int pos = new(x, y);
                    if (visited.Contains(pos))
                    {
                        continue;
                    }
                    int count = JunctionPredictorAroundCount(pos, size, junctionPredictor);
                    if (count == 2)
                    {
                        HandleTrackEdge(pos, size, junctionPredictor, trackNodes, trackEdges, visited);
                    }
                }
            }

            for (int x = 0; x < junctionPredictor.GetLength(0); x++)
            {
                for (int y = 0; y < junctionPredictor.GetLength(1); y++)
                {
                    if (junctionPredictor[x, y] == 2)
                    {
                        if (nodeToJunction.TryGetValue(new(x, y), out Structure str))
                        {
                            ResolveJunctionEdges(new(x, y), junctionPredictor, str, junctionT, trackNodes, trackEdges);
                        }
                    }
                }
            }

            return new WorldGenerationResult { DebugTexts = debugTexts, OccupiedMap = _occupiedMap, JunctionPredictor = junctionPredictor, Structures = structures, Tiles = tiles, TrackPaths = null, TrackEdges = trackEdges, TrackNodes = trackNodes };
        }

        private void GenerateExtraJunctions(int[,] junctionPredictor, Vector2Int size, OccupiedMap<IPlaceable> occupiedMap, List<DebugText> debugTexts)
        {
            int[,] distanceField = ComputeJunctionDistanceField(junctionPredictor, size);
            List<DistanceInfo> distanceInfo = GetDistanceInfo(distanceField);

            HashSet<Vector2Int> visited = new();
            while (distanceInfo.Count > 0)
            {
                DistanceInfo info = distanceInfo[0];
                distanceInfo.RemoveAt(0);
                if (info.Distance < 20) //Minimum distance between junctions
                {
                    continue;
                }
                if (visited.Contains(info.Position))
                {
                    continue;
                }

                debugTexts.Add(new DebugText()
                {
                    Position = info.Position,
                    Text = $"{info.Distance}"
                });
                Queue<Vector2Int> queue = new();
                HashSet<Vector2Int> onEdge = new();
                queue.Enqueue(info.Position);

                while (queue.Count > 0)
                {
                    Vector2Int pos = queue.Dequeue();
                    if (onEdge.Contains(pos))
                    {
                        continue;
                    }
                    onEdge.Add(pos);

                    for (int i = 0; i < Util.CardinalOffsets.Length; i++)
                    {
                        Vector2Int next = Util.CardinalOffsets[i] + pos;
                        if (next.x < 0 || next.y < 0 || next.x > size.x - 1 || next.y > size.y - 1)
                        {
                            continue;
                        }
                        if (junctionPredictor[next.x, next.y] != 1)
                        {
                            continue;
                        }
                        queue.Enqueue(next);
                    }
                }

                foreach (Vector2Int pos in onEdge)
                {
                    visited.Add(pos);
                }

                Vector2Int startPosition = info.Position;
                TryAddJunction(startPosition, size, junctionPredictor, distanceField, occupiedMap, debugTexts);
            }
        }

        private void TryAddJunction(Vector2Int pos, Vector2Int size, int[,] junctionPredictor, int[,] distanceField, OccupiedMap<IPlaceable> occupiedMap, List<DebugText> debugTexts)
        {
            bool successAdding = false;
            for (int i = 0; i < Util.CardinalOffsets.Length; i++)
            {
                if (successAdding)
                {
                    break;
                }
                Vector2Int offset = Util.CardinalOffsets[i];
                Vector2Int next = pos;

                bool hasSeenEmpty = false;
                int fullCount = 0;
                while (true)
                {
                    if (successAdding)
                    {
                        break;
                    }
                    next += offset;
                    if (next.x < 0 || next.y < 0 || next.x > size.x - 1 || next.y > size.y - 1)
                    {
                        break;
                    }
                    if (occupiedMap.IsOccupied(next))
                    {
                        break;
                    }

                    if (hasSeenEmpty)
                    {
                        if (junctionPredictor[next.x, next.y] == 1)
                        {
                            if (distanceField[next.x, next.y] < 20)//Minimum distance
                            {
                                break;
                            }
                            successAdding = true;
                            junctionPredictor[pos.x, pos.y] += 1;
                            junctionPredictor[next.x, next.y] += 1;

                            Vector2Int goal = next;
                            while (true)
                            {
                                next -= offset;
                                if (next == pos)
                                {
                                    debugTexts.Add(new DebugText()
                                    {
                                        Position = goal,
                                        Text = $"Meet: {pos} {goal}"
                                    });
                                    break;
                                }

                                junctionPredictor[next.x, next.y] += 1;
                            }
                        }

                    }
                    else
                    {
                        if (fullCount > 0)
                        {
                            break;
                        }
                        fullCount += 1;
                    }
                    if (junctionPredictor[next.x, next.y] == 0)
                    {
                        hasSeenEmpty = true;
                    }
                }
            }
        }

        struct DistanceInfo
        {
            public Vector2Int Position;
            public int Distance;
        }
        private List<DistanceInfo> GetDistanceInfo(int[,] distanceField)
        {
            List<DistanceInfo> distanceInfo = new();
            for (int x = 0; x < distanceField.GetLength(0); x++)
            {
                for (int y = 0; y < distanceField.GetLength(1); y++)
                {
                    if (distanceField[x, y] != 0)
                    {
                        distanceInfo.Add(new DistanceInfo()
                        {
                            Distance = distanceField[x, y],
                            Position = new(x, y)
                        });
                    }
                }
            }

            distanceInfo.Sort((a, b) => b.Distance.CompareTo(a.Distance));
            return distanceInfo;
        }

        private int[,] ComputeJunctionDistanceField(int[,] junctionPredictor, Vector2Int size)
        {
            int[,] distanceField = new int[size.x, size.y];
            Queue<Vector2Int> queue = new();

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    int count = JunctionPredictorAroundCount(new(x, y), size, junctionPredictor);
                    if (count == 1 && (junctionPredictor[x, y] == 1 || junctionPredictor[x, y] == -1))
                    {
                        queue.Enqueue(new(x, y));
                        distanceField[x, y] = 1;
                    }
                    if (junctionPredictor[x, y] == 2)
                    {
                        queue.Enqueue(new(x, y));
                        distanceField[x, y] = 1;
                    }
                    if (x == 0 || y == 0 || x == size.x - 1 || y == size.y - 1)
                    {
                        if (junctionPredictor[x, y] != 0)
                        {
                            queue.Enqueue(new(x, y));
                            distanceField[x, y] = 1;
                        }
                    }
                }
            }

            HashSet<Vector2Int> visited = new();
            while (queue.Count > 0)
            {
                Vector2Int pos = queue.Dequeue();
                if (visited.Contains(pos))
                {
                    continue;
                }
                visited.Add(pos);

                int nextDistance = distanceField[pos.x, pos.y] + 1;
                for (int i = 0; i < Util.CardinalOffsets.Length; i++)
                {
                    Vector2Int offset = Util.CardinalOffsets[i];
                    Vector2Int next = pos + offset;
                    if (next.x < 0 || next.y < 0 || next.x > size.x - 1 || next.y > size.y - 1)
                    {
                        continue;
                    }
                    if (junctionPredictor[next.x, next.y] != 0 && distanceField[next.x, next.y] == 0)
                    {
                        distanceField[next.x, next.y] = nextDistance;
                        queue.Enqueue(next);
                    }
                }
            }

            return distanceField;
        }

        private void ResolveJunctionEdges(Vector2Int node, int[,] junctionPredictor, Structure junction, StructureBlueprintTrack sbt, List<Vector2Int> trackNodes, List<(Vector2Int, Vector2Int)> trackEdges)
        {
            List<(Vector2Int, Vector2Int)> edges = new();
            List<(Vector2Int, Vector2Int)> toRemove = new();
            for (int i = trackEdges.Count - 1; i >= 0; i--)
            {
                (Vector2Int, Vector2Int) edge = trackEdges[i];
                Vector2Int a = edge.Item1;
                Vector2Int b = edge.Item2;

                if (a == node)
                {
                    edges.Add((a, b));
                    trackEdges.RemoveAt(i);
                }
                if (b == node)
                {
                    edges.Add((b, a));
                    trackEdges.RemoveAt(i);
                }
            }

            trackNodes.Remove(node);

            Rotation rot = JunctionRotation(node, junctionPredictor);
            for (int i = 0; i < edges.Count; i++)
            {
                (Vector2Int, Vector2Int) edge = edges[i];
                Vector2Int junctionNode = edge.Item1;
                Vector2Int otherNode = edge.Item2;

                float closestDist = float.MaxValue;
                Vector2Int closestCanditate = Vector2Int.zero;
                TrackPath[] trackPaths = sbt.TrackPaths;
                for (int j = 0; j < trackPaths.Length; j++)
                {
                    Vector2Int[] path = trackPaths[j].Path;
                    for (int k = 0; k < path.Length; k++)
                    {
                        Vector2Int pos = RotationManager.Rotate(path[k] + trackPaths[j].CenterOffset, rot) + junction.WorldPosition;

                        float dist = DistanceSqrd(otherNode, pos);
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            closestCanditate = pos;
                        }
                    }
                }

                trackEdges.Add((otherNode, closestCanditate));
            }
        }

        private float DistanceSqrd(Vector2Int a, Vector2Int b)
        {
            int dx = a.x - b.x;
            int dy = a.y - b.y;
            return dx * dx + dy * dy;
        }

        private void HandleTrackEdge(Vector2Int start, Vector2Int size, int[,] junctionPredictor, List<Vector2Int> trackNodes, List<(Vector2Int, Vector2Int)> trackEdges, HashSet<Vector2Int> visited)
        {
            Queue<Vector2Int> queue = new();
            queue.Enqueue(start);

            List<Vector2Int> nodes = new();
            while (queue.Count > 0 && nodes.Count < 2)
            {
                Vector2Int node = queue.Dequeue();
                if (visited.Contains(node))
                {
                    continue;
                }

                if (junctionPredictor[node.x, node.y] == 2)
                {
                    nodes.Add(node);
                    continue;
                }
                visited.Add(node);

                int count = JunctionPredictorAroundCount(node, size, junctionPredictor);
                if (count == 1)
                {
                    nodes.Add(node);
                    continue;
                }

                for (int i = 0; i < Util.CardinalOffsets.Length; i++)
                {
                    Vector2Int pos = Util.CardinalOffsets[i] + node;
                    if (pos.x < 0 || pos.y < 0 || pos.x > size.x - 1 || pos.y > size.y - 1)
                    {
                        continue;
                    }
                    if (junctionPredictor[pos.x, pos.y] == 0)
                    {
                        continue;
                    }
                    queue.Enqueue(pos);
                }
            }

            if (nodes.Count == 0)
            {
                return;
            }
            if (nodes.Count == 1)
            {
                Debug.Log($"{start} {nodes[0]}");
                return;
            }

            trackNodes.Add(nodes[0]);
            trackNodes.Add(nodes[1]);
            trackEdges.Add((nodes[0], nodes[1]));
        }
        private int JunctionPredictorAroundCount(Vector2Int pos, Vector2Int size, int[,] junctionPredictor)
        {
            int count = 0;
            for (int i = 0; i < Util.CardinalOffsets.Length; ++i)
            {
                Vector2Int node = pos + Util.CardinalOffsets[i];
                if (node.x < 0 || node.y < 0 || node.x > size.x - 1 || node.y > size.y - 1)
                {
                    continue;
                }

                if (junctionPredictor[node.x, node.y] != 0)
                {
                    count += 1;
                }
            }
            return count;
        }

        private Rotation JunctionRotation(Vector2Int node, int[,] junctionPredictor)
        {
            for (int i = 0; i < Util.CardinalOffsets.Length; i++)
            {
                Vector2Int offset = Util.CardinalOffsets[i];
                Vector2Int pos = offset + node;
                if (junctionPredictor[pos.x, pos.y] == 0)
                {
                    Rotation rot = RotationManager.FlipRotation(RotationManager.CardinalOffsetToDirection(offset));
                    return rot;
                }
            }

            return Rotation.Up;
        }

        private Structure AddJunctionAndEdges(Vector2Int originalPosition, Vector2Int junctionPos, Vector2Int size, StructureBlueprint tJB, StructureBlueprintTrack junctionT, List<Structure> structures, OccupiedMap<IPlaceable> occupiedMap, List<DebugText> debugTexts, int[,] junctionPredictor, List<Vector2Int> trackNodes, List<(Vector2Int, Vector2Int)> trackEdges, List<Vector2Int[]> trackPointsList)
        {
            Rotation rot = JunctionRotation(originalPosition, junctionPredictor);

            debugTexts.Add(new DebugText()
            {
                Position = originalPosition,
                Text = $"Empty {rot}"
            });

            Structure tJS = new(tJB, junctionPos, rot);
            if (occupiedMap.CanPlace(tJS))
            {
                occupiedMap.Add(tJS);
                structures.Add(tJS);
                ExpandTrackPath(junctionPos, junctionT.TrackPaths, rot, trackPointsList, trackNodes, trackEdges);
                return tJS;
                /*
                for (int j = 0; j < junctionT.TrackPaths.Length; j++)
                {
                    TrackPath trackPath = junctionT.TrackPaths[j];
                    Vector2Int centerOffset = trackPath.CenterOffset;
                    Vector2Int start = RotationManager.Rotate(trackPath.Path[^1] + centerOffset, rot);
                    Vector2Int seccondLast = RotationManager.Rotate(trackPath.Path[^2] + centerOffset, rot);
                    Vector2Int searchPos = start + junctionPos;
                    Vector2Int moveOffset = start - seccondLast;
                    moveOffset = new(Sign(moveOffset.x), Sign(moveOffset.y));
                    while (true)
                    {
                        if (searchPos.x < 0 || searchPos.y < 0 || searchPos.x > size.x - 1 || searchPos.y > size.y - 1)
                        {
                            break;
                        }
                        if (junctionPredictor[searchPos.x, searchPos.y] == 2)
                        {
                            break;
                        }
                        if (junctionPredictor[searchPos.x, searchPos.y] == 0)
                        {
                            break;
                        }
                        searchPos += moveOffset;
                    }

                    debugTexts.Add(new DebugText()
                    {
                        Text = $"{start} {moveOffset}",
                        Position = junctionPos + moveOffset,
                    });
                    trackEdges.Add((start + junctionPos, searchPos));
                }
                */
            }
            return null;
        }
        struct JunctionInfo
        {
            public Vector2Int Position;
            public List<Vector2Int> Edges;
        }

        private static int Sign(int x)
        {
            if (x < 0)
            {
                return -1;
            }
            if (x > 0)
            {
                return 1;
            }
            return 0;
        }

        private void ExpandTrackPath(Vector2Int gridPos, TrackPath[] trackPaths, Rotation rot, List<Vector2Int[]> trackPointsList, List<Vector2Int> trackNodes, List<(Vector2Int, Vector2Int)> trackEdges)
        {
            for (int l = 0; l < trackPaths.Length; l++)
            {
                Vector2Int[] path = trackPaths[l].Path;
                Vector2Int centerOffset = trackPaths[l].CenterOffset;
                Vector2Int[] trackPoints = new Vector2Int[path.Length];
                trackPointsList.Add(trackPoints);

                for (int k = 0; k < trackPoints.Length; k++)
                {
                    trackPoints[k] = RotationManager.Rotate(path[k] + centerOffset, rot) + gridPos;
                    trackNodes.Add(trackPoints[k]);
                }
                for (int k = 0; k < trackPoints.Length - 1; k++)
                {
                    trackEdges.Add((trackPoints[k], trackPoints[k + 1]));
                }
            }
        }

        public class BoundFiller : IPlaceable
        {
            Vector2Int[] _pos;
            public BoundFiller(Vector2Int pos)
            {
                _pos = new Vector2Int[1] { pos };
            }
            public Vector2Int[] GetOccupiedPositions()
            {
                return _pos;
            }
        }

        private void ExpandStructureBounds(Vector2Int[] bounds, Vector2Int size, OccupiedMap<IPlaceable> occupiedMap, List<BoundFiller> fillers)
        {
            List<Vector2Int> visited = new();
            for (int i = 0; i < bounds.Length; i++)
            {
                for (int j = 0; j < Util.CardinalOffsets.Length; j++)
                {
                    Vector2Int offset = Util.CardinalOffsets[j];

                    Vector2Int newPos = bounds[i];
                    while (true)
                    {
                        newPos += offset;
                        if (newPos.x < 0 || newPos.y < 0 || newPos.x > size.x - 1 || newPos.y > size.y - 1)
                        {
                            break;
                        }
                        if (visited.Contains(newPos))
                        {
                            break;
                        }
                        visited.Add(newPos);

                        if (!occupiedMap.IsOccupied(newPos))
                        {
                            BoundFiller filler = new(newPos);
                            occupiedMap.Add(filler);
                            fillers.Add(filler);
                        }
                    }
                }
            }
        }

        private void AddPathsToLostStructures(int[,] junctionPredictor)
        {
            for (int x = 0; x < junctionPredictor.GetLength(0); x++)
            {
                for (int y = 0; y < junctionPredictor.GetLength(1); y++)
                {
                    if (junctionPredictor[x, y] == 1)
                    {
                    }
                }
            }
        }

        class Junction : IPlaceable
        {
            private readonly Vector2Int[] _occupiedPositions;
            public Junction(Vector2Int[] occupiedPositions)
            {
                _occupiedPositions = occupiedPositions;
            }
            public Vector2Int[] GetOccupiedPositions()
            {
                return _occupiedPositions;
            }
        }

        private Vector2Int MoveJunction(Vector2Int node, Rotation rot, StructureBlueprint jbt, OccupiedMap<IPlaceable> occupiedMap, Vector2Int size)
        {
            int i = 10;
            while (i-- > 0)
            {
                int randX = Random.Range(-10, +11);
                int randY = Random.Range(-10, +11);
                if (rot == Rotation.Right && randX < 0)
                {
                    continue;
                }
                if (rot == Rotation.Left && randX > 0)
                {
                    continue;
                }
                if (rot == Rotation.Up && randY < 0)
                {
                    continue;
                }
                if (rot == Rotation.Down && randY > 0)
                {
                    continue;
                }

                int x = node.x + randX;
                int y = node.y + randY;

                if (x < 0 || y < 0 || x > size.x - 1 || y > size.y - 1)
                {
                    continue;
                }

                Structure structure = new(jbt, new(x, y));
                if (!occupiedMap.CanPlace(structure))
                {
                    continue;
                }

                return new(x, y);
            }

            return node;
        }

        private void MoveJunctionNode(int nodeIndex, List<Vector2Int> trackNodes, OccupiedMap<IPlaceable> occupiedMap, Vector2Int size)
        {
            Vector2Int node = new(trackNodes[nodeIndex].x, trackNodes[nodeIndex].y);
            int i = 10;
            while (i-- > 0)
            {
                bool failed = false;
                int randX = Random.Range(-10, +11);
                int randY = Random.Range(-10, +11);
                int x = node.x + randX;
                int y = node.y + randY;

                if (x < 0 || y < 0 || x > size.x - 1 || y > size.y - 1)
                {
                    continue;
                }

                List<Vector2Int> occupiedPositions = new();
                for (int dx = -10; dx <= 10; dx++)
                {
                    if (failed)
                    {
                        break;
                    }
                    for (int dy = -10; dy <= 10; dy++)
                    {
                        int nx = x + dx;
                        int ny = y + dy;
                        if (_occupiedMap.IsOccupied(new(nx, ny)))
                        {
                            failed = true;
                            break;
                        }
                        occupiedPositions.Add(new(nx, ny));
                    }
                }
                if (failed)
                {
                    continue;
                }
                Junction jn = new(occupiedPositions.ToArray());
                if (!occupiedMap.CanPlace(jn))
                {
                    continue;
                }

                occupiedMap.Add(jn);
                trackNodes[nodeIndex] = new(x, y);
                return;
            }

            trackNodes.RemoveAt(nodeIndex);
        }

        public void Initialize(IServiceRegistry serviceRegistry)
        {
            _trackSegmentManager = serviceRegistry.GetService<TrackSegmentManager>();
            _worldSettings = serviceRegistry.GetService<WorldSettings>();
            _structureBlueprintManager = serviceRegistry.GetService<StructureBlueprintManager>();
        }
    }
}