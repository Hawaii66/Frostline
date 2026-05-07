using NaughtyAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Runner : MonoBehaviour
{
    Graph graph;
    TrackGenerator trackGenerator;
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
        Vector2Int start = graph.edges[1].a.polar.ToCartesianInt();
        Vector2Int end = graph.edges[1].b.polar.ToCartesianInt();

        trackGenerator = new TrackGenerator();

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
    }
}
