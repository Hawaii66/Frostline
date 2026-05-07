using System.Collections.Generic;
using UnityEngine;

public abstract class Node
{
    public Polar polar;
    public List<Node> edges;

    public Node(Polar polar)
    {
        this.polar = polar;
        edges = new List<Node>();
    }

    public void AddEdge(Node node)
    {
        if (!edges.Contains(node))
        {
            edges.Add(node);
        }
    }
    public int ConnectedNodes()
    {
        HashSet<string> seen = new();
        for (int i = 0; i < edges.Count; ++i)
        {
            Node node = edges[i];
            seen.Add(node.polar.ToCartesian().ToString());
        }
        return seen.Count;
    }

    public abstract Color GetColor();
    public abstract bool AllowsJunctionConnections();
}