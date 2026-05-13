using Frostline.Core;
using Frostline.World;
using Frostline.World.Generation;
using Frostline.World.Structures;
using Frostline.World.Tiles;
using Frostline.World.Tracks;
using System.Collections.Generic;
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

            _occupiedMap = new();
            _structureBlueprintManager.TryGetStructureBlueprint("Cube_SB", out StructureBlueprint cubeBlueprint);
            _structureBlueprintManager.TryGetStructureBlueprint("Center_SB", out StructureBlueprint centerBlueprint);

            Vector2Int center = size / 2;
            Structure centerStructure = new(centerBlueprint, center);
            if (!_occupiedMap.CanPlace(centerStructure))
            {
                Debug.LogError($"Failed to place center structures");
                return null;
            }
            _occupiedMap.Add(centerStructure);
            structures.Add(centerStructure);

            int structureCount = 10;
            for (int i = 0; i < structureCount; i++)
            {
                int maxAttempts = 20;
                for (int j = 0; j < maxAttempts; j++)
                {
                    int randX = Random.Range(0, size.x);
                    int randY = Random.Range(0, size.y);

                    Structure cubeStructure = new(cubeBlueprint, new Vector2Int(randX, randY));
                    if (_occupiedMap.CanPlace(cubeStructure))
                    {
                        _occupiedMap.Add(cubeStructure);
                        structures.Add(cubeStructure);
                        break;
                    }
                }
            }

            return new WorldGenerationResult { Structures = structures, Tiles = tiles, TrackPaths = null };
        }

        public void Initialize(IServiceRegistry serviceRegistry)
        {
            _trackSegmentManager = serviceRegistry.GetService<TrackSegmentManager>();
            _worldSettings = serviceRegistry.GetService<WorldSettings>();
            _structureBlueprintManager = serviceRegistry.GetService<StructureBlueprintManager>();
        }
    }
}