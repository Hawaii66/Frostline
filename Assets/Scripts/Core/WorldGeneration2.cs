using Frostline.Core;
using Frostline.World;
using Frostline.World.Structures;
using Frostline.World.Tiles;
using Frostline.World.Tracks;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Frostline.Test
{

    public class TrackPath
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
        public List<TrackPath> TrackPaths;
        public List<Vector2Int> TrackNodes;
        public List<(Vector2Int, Vector2Int)> TrackEdges;
        public int[,] JunctionPredictor;
        public OccupiedMap<IPlaceable> OccupiedMap;
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

            _occupiedMap = new();
            _structureBlueprintManager.TryGetStructureBlueprint("TestT_SB", out StructureBlueprint cubeBlueprint);
            _structureBlueprintManager.TryGetStructureBlueprint("Center_SB", out StructureBlueprint centerBlueprint);
            _trackSegmentManager.TrackSegmentDict.TryGetValue("TestT", out StructureBlueprintTrack trackT);

            Vector2Int center = size / 2;
            Structure centerStructure = new(centerBlueprint, center);
            if (!_occupiedMap.CanPlace(centerStructure.GetBounds()))
            {
                Debug.LogError($"Failed to place center structures");
                return null;
            }
            _occupiedMap.Add(centerStructure, centerStructure.GetBounds());
            structures.Add(centerStructure);

            int[,] junctionPredictor = new int[size.x, size.y];
            List<Vector2Int[]> trackPointsList = new();

            int structureCount = 10;
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

                        Vector2Int[] trackPoints = new Vector2Int[trackT.TrackSegments.Length];
                        trackPointsList.Add(trackPoints);

                        for (int k = 0; k < trackPoints.Length; k++)
                        {
                            trackPoints[k] = RotationManager.Rotate(trackT.TrackSegments[k] + trackT.CenterOffset, rot) + gridPos;
                            trackNodes.Add(trackPoints[k]);
                        }
                        for (int k = 0; k < trackPoints.Length - 1; k++)
                        {
                            trackEdges.Add((trackPoints[k], trackPoints[k + 1]));
                        }

                        break;
                    }
                }
            }

            foreach (Vector2Int[] trackPoints in trackPointsList)
            {
                Vector2Int diffTrackPoints = trackPoints[trackPoints.Length - 1] - trackPoints[trackPoints.Length - 2];
                Vector2Int pos = trackPoints[trackPoints.Length - 1];
                junctionPredictor[pos.x, pos.y] = -1;
                pos += diffTrackPoints;

                while (true)
                {
                    if (pos.x < 0 || pos.x > size.x - 1 || pos.y < 0 || pos.y > size.y - 1) { break; }
                    if (_occupiedMap.IsOccupied(pos)) { break; }

                    junctionPredictor[pos.x, pos.y] += 1;
                    //if (junctionPredictor[pos.x, pos.y] > 1) { break; }
                    pos += diffTrackPoints;
                }
            }

            JunctionNodes(junctionPredictor, trackNodes);

            for (int x = 0; x < junctionPredictor.GetLength(0); x++)
            {
                for (int y = 0; y < junctionPredictor.GetLength(1); y++)
                {
                    if (junctionPredictor[x, y] > 1 && trackNodes.Contains(new(x, y)))
                    {
                        int index = trackNodes.IndexOf(new(x, y));
                        MoveJunctionNode(index, trackNodes, _occupiedMap, size);
                    }
                }
            }

            return new WorldGenerationResult { OccupiedMap = _occupiedMap, JunctionPredictor = junctionPredictor, Structures = structures, Tiles = tiles, TrackPaths = null, TrackEdges = trackEdges, TrackNodes = trackNodes };
        }

        private void JunctionNodes(int[,] junctionPredictor, List<Vector2Int> trackNodes)
        {
            for (int x = 0; x < junctionPredictor.GetLength(0); x++)
            {
                for (int y = 0; y < junctionPredictor.GetLength(1); y++)
                {
                    if (junctionPredictor[x, y] == 2)
                    {
                        trackNodes.Add(new(x, y));
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