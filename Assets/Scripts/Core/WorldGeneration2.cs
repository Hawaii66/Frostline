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
    }

    public class WorldGeneration2 : IRequireServices
    {
        private TrackSegmentManager _trackSegmentManager;
        private StructureBlueprintManager _structureBlueprintManager;
        private WorldSettings _worldSettings;
        private OccupiedMap<Structure> _occupiedMap;

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

            return new WorldGenerationResult { JunctionPredictor = junctionPredictor, Structures = structures, Tiles = tiles, TrackPaths = null, TrackEdges = trackEdges, TrackNodes = trackNodes };
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
                    //if (junctionPredictor[x, y] == 1)
                    //{
                    //    JunctionNodesExpand(junctionPredictor, trackNodes, new(x, y));
                    //}
                }
            }
        }

        private void JunctionNodesExpand(int[,] junctionPredictor, List<Vector2Int> trackNodes, Vector2Int start)
        {
            Queue<Vector2Int> toScan = new();
            HashSet<Vector2Int> seen = new();
            toScan.Enqueue(start);

            Vector2Int[] offsets = new Vector2Int[]
            {
                new(1,0),
                new(0,1),
                new(-1,0),
                new(0,-1),
            };

            Vector2Int finalPosition = Vector2Int.zero;
            while (toScan.Count > 0)
            {
                Vector2Int pos = toScan.Dequeue();
                if (seen.Contains(pos)) { continue; }
                seen.Add(pos);
                if (junctionPredictor[pos.x, pos.y] > 1)
                {
                    return;
                }
                if (junctionPredictor[pos.x, pos.y] == -1)
                {
                    continue;
                }
                finalPosition = pos;
                for (int i = 0; i < offsets.Length; i++)
                {
                    Vector2Int next = pos + offsets[i];
                    if (next.x < 0 || next.y < 0 || next.x > junctionPredictor.GetLength(0) - 1 || next.y > junctionPredictor.GetLength(1) - 1)
                    {
                        continue;
                    }
                    if (junctionPredictor[next.x, next.y] == 0)
                    {
                        continue;
                    }
                    toScan.Enqueue(next);
                }
            }

            foreach (Vector2Int pos in seen)
            {
                junctionPredictor[pos.x, pos.y] = 0;
            }
            if (finalPosition != Vector2.zero)
            {
                junctionPredictor[finalPosition.x, finalPosition.y] = 2;
            }
        }

        public void Initialize(IServiceRegistry serviceRegistry)
        {
            _trackSegmentManager = serviceRegistry.GetService<TrackSegmentManager>();
            _worldSettings = serviceRegistry.GetService<WorldSettings>();
            _structureBlueprintManager = serviceRegistry.GetService<StructureBlueprintManager>();
        }
    }
}