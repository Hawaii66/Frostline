using Frostline.World;
using Frostline.World.Generation;
using Frostline.World.Structures;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Frostline.DEBUG
{
    public class Runner : MonoBehaviour
    {
        Graph graph;
        WorldContext world;
        List<List<Vector2Int>> trackPaths;
        Dictionary<string, StructureBlueprint> structureBlueprints;

        public GraphSettings settings;

        [Button("Generate Graph")]
        void Run()
        {
            graph = GraphGenerator.Generate(settings);
        }

        [Button("Generate Rails")]
        void Run2()
        {
            TrackGenerator trackGenerator = new();

            trackPaths = new List<List<Vector2Int>>();
            for (int i = 0; i < graph.Edges.Count; i++)
            {
                Edge edge = graph.Edges[i];
                TrackResult trackResult = trackGenerator.Generate(edge.A.Polar.ToCartesianInt(), edge.B.Polar.ToCartesianInt());
                if (trackResult.success)
                {
                    trackPaths.Add(trackResult.path);
                }
                else
                {
                    Debug.Log("Path failed to generate");
                }
            }
        }

        [Button("Load structures")]
        void RunStructureLoad()
        {
            structureBlueprints = new();

            string[] guids = AssetDatabase.FindAssets($"t:{typeof(StructureBlueprint).Name}", new[] { "Assets/Frostline/Structures" });
            StructureBlueprint[] filtered = guids.Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<StructureBlueprint>)
                .ToArray();

            for (int i = 0; i < filtered.Length; i++)
            {
                structureBlueprints.Add(filtered[i].name, filtered[i]);
            }
        }

        [Button("Generate world")]
        void Run3()
        {
            world = new WorldContext();
            StructureBlueprint structureBlueprint = StructureBlueprint.New(new Vector2Int[]
            {
            new (0,0),
            new (1,0),
            new (1,1),
            new (0,1),
            });
            Structure structure = new(structureBlueprint, Vector2Int.zero);
            world.TryAddStructure(structure);

            Structure structure2 = new(structureBlueprint, Vector2Int.right);
            world.TryAddStructure(structure2);

            StructureBlueprint structureBlueprint2 = StructureBlueprint.New(new Vector2Int[]
            {
            new (0,0),
            new (1,0),
            new (1,1),
            new (0,1),
            });
            Structure structure3 = new(structureBlueprint2, Vector2Int.right * 3 + Vector2Int.up * 2);
            world.TryAddStructure(structure3);

            if (structureBlueprints.TryGetValue("Cube", out StructureBlueprint sb))
            {
                world.TryAddStructure(new(sb, new Vector2Int(20, 20)));
            }
        }

        private void OnDrawGizmos()
        {
            if (graph != null)
            {
                Node[] nodes = graph.Nodes.ToArray();
                for (int i = 0; i < nodes.Length; i++)
                {
                    Node node = nodes[i];
                    Gizmos.color = node.GetColor();
                    Gizmos.DrawSphere(node.Polar.ToCartesian3(), 0.2f);
                    if (node is JunctionNode junctionNode)
                    {
                        Handles.Label(node.Polar.ToCartesian3() + Vector3.up, "Edges: " + junctionNode.ConnectedNodes());
                    }
                }

                Gizmos.color = Color.blue;
                Edge[] edges = graph.Edges.ToArray();
                for (int i = 0; i < edges.Length; i++)
                {
                    Vector3 a = edges[i].A.Polar.ToCartesian3();
                    Vector3 b = edges[i].B.Polar.ToCartesian3();
                    Gizmos.DrawLine(a, b);
                }

                Vector3 start = new(graph.NegativeOffset.x, 0, graph.NegativeOffset.y);
                Vector3 end = new(graph.NegativeOffset.x + graph.SizeX, 1, graph.NegativeOffset.y + graph.SizeY);
                Vector3 center = (start + end) / 2f;
                Vector3 size = end - start;
                Gizmos.DrawWireCube(center, size);
            }

            if (trackPaths != null && trackPaths.Count > 0)
            {
                Gizmos.color = Color.darkGoldenRod;
                for (int i = 0; i < trackPaths.Count; i++)
                {
                    for (int j = 0; j < trackPaths[i].Count - 1; j++)
                    {
                        Vector3 start = new Vector3(trackPaths[i][j].x, 0, trackPaths[i][j].y);
                        Vector3 end = new Vector3(trackPaths[i][j + 1].x, 0, trackPaths[i][j + 1].y);

                        Gizmos.DrawLine(start, end);
                    }
                }
            }

            if (world != null)
            {
                Gizmos.color = Color.black;
                for (int i = 0; i < world.Structures.Count; i++)
                {
                    Structure structure = world.Structures[i];
                    for (int j = 0; j < structure.GetOccupiedPositions().Length; j++)
                    {
                        Vector2Int position = structure.GetOccupiedPositions()[j];
                        Gizmos.DrawWireSphere(new Vector3(position.x, 0, position.y), 0.2f);
                    }
                }
            }
        }
    }
}