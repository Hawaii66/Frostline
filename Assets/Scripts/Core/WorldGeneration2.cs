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

            for (int x = 0; x < junctionPredictor.GetLength(0); x++)
            {
                for (int y = 0; y < junctionPredictor.GetLength(1); y++)
                {
                    if (junctionPredictor[x, y] == 2)
                    {
                        Rotation rot = JunctionRotation(new(x, y), junctionPredictor);
                        Vector2Int movedPosition = MoveJunction(new(x, y), rot, tJunctionBlueprint, _occupiedMap, size);
                        AddJunctionAndEdges(new(x, y), movedPosition, size, tJunctionBlueprint, junctionT, structures, _occupiedMap, debugTexts, junctionPredictor, trackNodes, trackEdges, trackPointsList);
                    }
                }
            }

            return new WorldGenerationResult { DebugTexts = debugTexts, OccupiedMap = _occupiedMap, JunctionPredictor = junctionPredictor, Structures = structures, Tiles = tiles, TrackPaths = null, TrackEdges = trackEdges, TrackNodes = trackNodes };
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

        private void AddJunctionAndEdges(Vector2Int originalPosition, Vector2Int junctionPos, Vector2Int size, StructureBlueprint tJB, StructureBlueprintTrack junctionT, List<Structure> structures, OccupiedMap<IPlaceable> occupiedMap, List<DebugText> debugTexts, int[,] junctionPredictor, List<Vector2Int> trackNodes, List<(Vector2Int, Vector2Int)> trackEdges, List<Vector2Int[]> trackPointsList)
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