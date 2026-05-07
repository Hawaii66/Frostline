using NaughtyAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Runner : MonoBehaviour
{
    Graph graph;
    List<Vector2Int> trackPath;

    public GraphSettings settings;

    [Button("Generate Graph")]
    void Run()
    {
        graph = GraphGenerator.Generate(settings);
    }

    [Button("Generate Rails")]
    void Run2()
    {
        Vector2Int start = graph.edges[1].a.polar.ToCartesianInt();
        Vector2Int end = graph.edges[1].b.polar.ToCartesianInt();
        trackPath = Track.Generate(start, end);
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
        }

        if (trackPath.Count > 0)
        {
            Gizmos.color = Color.darkGoldenRod;
            for (int i = 0; i < trackPath.Count - 1; i++)
            {
                Vector3 start = new Vector3(trackPath[i].x, 0, trackPath[i].y);
                Vector3 end = new Vector3(trackPath[i + 1].x, 0, trackPath[i + 1].y);

                Gizmos.DrawLine(start, end);
            }
        }
    }
}
