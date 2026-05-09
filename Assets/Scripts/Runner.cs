using NaughtyAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Runner : MonoBehaviour
{
    Graph graph;
    World world;
    List<List<Vector2Int>> trackPaths;

    public GraphSettings settings;

    [Button("Generate Graph")]
    void Run()
    {
        graph = GraphGenerator.Generate(settings);
    }

    [Button("Generate Rails")]
    void Run2()
    {
        TrackGenerator trackGenerator = new ();

        trackPaths = new List<List<Vector2Int>>();
        for(int i = 0; i < graph.edges.Count; i++)
        {
            Edge edge = graph.edges[i];
            TrackResult trackResult = trackGenerator.Generate(edge.a.polar.ToCartesianInt(), edge.b.polar.ToCartesianInt());
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

    [Button("Generate world")]
    void Run3()
    {
        world = new World(graph.SizeX, graph.SizeY);
        StructureBlueprint structureBlueprint = new StructureBlueprint(Vector2Int.zero, new Vector2Int[]
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(1,1),
            new Vector2Int(0,1),
        });
        Structure structure = new (structureBlueprint, Vector2Int.zero);
        world.TryAddStructure(structure);

        Structure structure2 = new (structureBlueprint, Vector2Int.right);
        world.TryAddStructure(structure2);

        StructureBlueprint structureBlueprint2 = new StructureBlueprint(Vector2Int.zero, new Vector2Int[]
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(1,1),
            new Vector2Int(0,1),
        });
        Structure structure3 = new Structure(structureBlueprint2, Vector2Int.right * 3 + Vector2Int.up * 2);
        world.TryAddStructure(structure3);
    }

    private void OnDrawGizmos()
    {
        if (graph != null)
        {
            Node[] nodes = graph.nodes.ToArray();
            for (int i = 0; i < nodes.Length; i++)
            {
                Node node = nodes[i];
                Gizmos.color = node.GetColor();
                Gizmos.DrawSphere(node.polar.ToCartesian3(), 0.2f);
                if (node is JunctionNode junctionNode)
                {
                    Handles.Label(node.polar.ToCartesian3() + Vector3.up, "Edges: " + junctionNode.ConnectedNodes());
                }
            }

            Gizmos.color = Color.blue;
            Edge[] edges = graph.edges.ToArray();
            for (int i = 0; i < edges.Length; i++)
            {
                Vector3 a = edges[i].a.polar.ToCartesian3();
                Vector3 b = edges[i].b.polar.ToCartesian3();
                Gizmos.DrawLine(a, b);
            }

            Vector3 start = new (graph.NegativeOffset.x, 0, graph.NegativeOffset.y);
            Vector3 end = new (graph.NegativeOffset.x + graph.SizeX, 1, graph.NegativeOffset.y + graph.SizeY);
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

        if(world != null)
        {
            Gizmos.color = Color.black;
            for (int i = 0; i < world.structures.Count; i++)
            {
                Structure structure = world.structures[i];
                for(int j = 0; j < structure.tiles.Length; j++)
                {
                    Vector2Int position = structure.tiles[j].position;
                    Gizmos.DrawWireSphere(new Vector3(position.x, 0, position.y), 0.2f);
                }
            }
        }
    }
}
