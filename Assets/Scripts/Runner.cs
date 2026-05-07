using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

public class Runner : MonoBehaviour
{
    Graph graph;

    public GraphSettings settings;

    [Button("Generate Graph")]
    void Run()
    {
        graph = GraphGenerator.Generate(settings);
    }

    [Button("Generate Rails")]
    void Run2()
    {
        Vector2 start = graph.edges[0].a.polar.ToCartesian();
        Vector2 end = graph.edges[0].b.polar.ToCartesian();
        Debug.Log(graph.edges[0].a.polar.ToCartesian() + " " + graph.edges[0].b.polar.ToCartesian());
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
                if(node is JunctionNode junctionNode)
                {
                    Handles.Label(node.polar.ToCartesian3() + Vector3.up, "Edges: " + junctionNode.ConnectedNodes());
                }
            }

            Gizmos.color = Color.blue;
            Edge[] edges = graph.edges.ToArray();
            for(int i = 0; i < edges.Length; i++)
            {
                Vector3 a = edges[i].a.polar.ToCartesian3();
                Vector3 b = edges[i].b.polar.ToCartesian3();
                Gizmos.DrawLine(a, b);
            }
        }
    }
}
