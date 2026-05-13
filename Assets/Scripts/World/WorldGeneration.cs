using Frostline.Core;
using Frostline.World.Generation;
using Frostline.World.Tiles;
using Frostline.World.Tracks;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Frostline.World
{
    public class WorldGeneration : IRequireServices
    {
        private WorldSettings _worldSettings;
        private TileManager _tileManager;
        private TrackSegmentManager _trackSegmentManager;
        public void Initialize(IServiceRegistry serviceRegistry)
        {
            _worldSettings = serviceRegistry.GetService<WorldSettings>();
            _tileManager = serviceRegistry.GetService<TileManager>();
            _trackSegmentManager = serviceRegistry.GetService<TrackSegmentManager>();
        }

        public void GenerateWorld()
        {
            Graph graph = GraphGenerator.Generate(_worldSettings.GraphSettings);
            TrackGenerator trackGenerator = new();

            List<List<Vector2Int>> trackPaths = new();
            Parallel.For(0, graph.Edges.Count, (int i) =>
            {
                System.Random r = new(_worldSettings.Seed + i);
                Edge edge = graph.Edges[i];
                TrackResult trackResult = trackGenerator.Generate(edge.A.Polar.ToCartesianInt(), edge.B.Polar.ToCartesianInt(), r);
                if (trackResult.success)
                {
                    trackPaths.Add(trackResult.path);
                    _trackSegmentManager.GenerateTrackSegments(edge, trackResult.path);
                }
                else
                {
                    Debug.Log("Path failed to generate");
                }
            });

            TrackNetwork trackNetwork = new(graph.SizeX, graph.SizeY);
            trackNetwork.SetTrackPaths(trackPaths);
            _trackSegmentManager.TrackPaths = trackPaths;

            _tileManager.GenerateTiles(graph.SizeX, graph.SizeY);
            _tileManager.ScaleHeights(trackNetwork);
        }
    }
}