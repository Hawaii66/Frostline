
using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    public List<Node> nodes;
    public List<Edge> edges;
    public int SizeX;
    public int SizeY;
    public Vector2 NegativeOffset;
    GraphSettings settings;

    public Graph(GraphSettings settings)
    {
        nodes = new List<Node>();
        edges = new List<Edge>();
        this.settings = settings;
    }

    public Vector2 ToWorldPosition(Polar polar)
    {
        return NegativeOffset + polar.ToCartesian();
    }

    bool TryAddNode(Node node, Node parent)
    {
        if (IsTooClose(node.polar))
        {
            return false;
        }

        Edge edge = new (parent, node);
        if (Edge.IsOverlapping(edges, edge))
        {
            return false;
        }

        nodes.Add(node);
        edges.Add(edge);
        parent.AddEdge(node);
        node.AddEdge(parent);
        return true;
    }
    public JunctionNode TryAddJunctionNode(Polar polar, Node parent, bool allowMakeConnections)
    {
        JunctionNode node = new (polar, settings, allowMakeConnections);
        if (TryAddNode(node, parent))
        {
            return node;
        }
        return null;
    }

    public EndNode TryAddEndNode(Polar polar, Node parent)
    {
        EndNode node = new(polar);
        if (TryAddNode(node, parent))
        {
            return node;
        }

        return null;
    }
    public void ReplaceNode(Node prev, Node next)
    {
        int index = nodes.IndexOf(prev);
        nodes[index] = next;
        for(int i = 0; i < edges.Count; i++)
        {
            Edge edge = edges[i];
            if(edge.a == prev)
            {
                edge.a = next;
            }
            if(edge.b == prev)
            {
                edge.b = next;
            }
        }
        for(int i = 0; i < prev.edges.Count; i++)
        {
            Node other = prev.edges[i];
            next.edges.Add(other);
            other.edges.Remove(prev);
            other.edges.Add(next);
        }
    }

    public void ExpandFromJunction(JunctionNode node, List<Node> toProcess)
    {
        Polar[] polars = node.GetNext();

        for (int i = 0; i < polars.Length; i++)
        {
            Polar polar = node.polar + polars[i];
            if (IsTooClose(polar))
            {
                continue;
            }

            if (Random.value < settings.EndNodeSpawnRate)
            {
                TryAddEndNode(polar, node);
            }
            else
            {
                JunctionNode newNode = TryAddJunctionNode(polar, node, true);
                if (newNode != null)
                {
                    toProcess.Add(newNode);
                }
            }
        }

        if (!node.AllowsJunctionConnections())
        {
            return;
        }
        Node[] closeNodes = GetNodesWithinDistance(node.polar, settings.JunctionConnectionSearchDistance, true);
        for(int i = 0; i < closeNodes.Length; i++)
        {
            Node other = closeNodes[i];
            if (!other.AllowsJunctionConnections())
            {
                continue;
            }
            if(Random.value > settings.JunctionConnectionChance)
            {
                continue;
            }

            Edge edge = new (node, other);
            if (HasEdge(edge))
            {
                continue;
            }
            if (Edge.IsOverlapping(edges, edge))
            {
                continue;
            }

            edges.Add(edge);
            node.AddEdge(other);
            other.AddEdge(node);
        }
    }

    bool HasEdge(Edge edge)
    {
        for(int i = 0;i < edges.Count; i++)
        {
            Edge other = edges[i];

            if(other.a == edge.a && other.b == edge.b)
            {
                return true;
            }
            if(other.a == edge.b && other.b == edge.a)
            {
                return true;
            }
        }

        return false;
    }

    bool IsTooClose(Polar polar)
    {
        Vector2 test = polar.ToCartesian();
        for(int i = 0; i < nodes.Count; i++)
        {
            Vector2 pos = nodes[i].polar.ToCartesian();

            float dist = Vector2.Distance(pos, test);
            if(dist < settings.MinDistanceBetweenNodes)
            {
                return true;
            }
        }

        return false;
    }

    Node[] GetNodesWithinDistance(Polar polar, float testDist, bool excludeSelf = false)
    {
        List<Node> closeNodes = new ();

        Vector2 test = polar.ToCartesian();
        for(int i = 0; i < nodes.Count; i++)
        {
            Vector2 pos = nodes[i].polar.ToCartesian();
            float dist = Vector2.Distance(pos, test);
            if(excludeSelf && dist == 0)
            {
                continue;
            }
            if(dist < testDist)
            {
                closeNodes.Add(nodes[i]);
            }
        }

        return closeNodes.ToArray();
    }
}
