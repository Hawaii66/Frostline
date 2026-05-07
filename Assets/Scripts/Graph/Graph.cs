
using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    public List<Node> nodes;
    public List<Edge> edges;

    GraphSettings settings;

    public Graph(GraphSettings settings)
    {
        nodes = new List<Node>();
        edges = new List<Edge>();
        this.settings = settings;
        Random.InitState(settings.seed);

        List<Node> toProcess = new();

        Debug.Log("Creating Start Circle");

        StartNode startNode = new (Polar.Zero);
        nodes.Add(startNode);

        float radSplit = Polar.CircleRad / settings.StartConnections;
        for(int i = 0; i < settings.StartConnections; i++)
        {
            float rad = radSplit * i + radSplit * settings.WithVariation(0,settings.StartRadVariation);
            float dist = settings.WithVariation(settings.StartDistance, settings.StartDistanceVariation);

            JunctionNode node = AddJunctionNode(new Polar(rad, dist), startNode, false);
            if(node != null)
            {
                toProcess.Add(node);
            }
        }

        Debug.Log("Generating Graph");

        while(toProcess.Count > 0 && nodes.Count <= settings.MaxNodes)
        {
            int randomIndex = Random.Range(0, toProcess.Count);
            Node node = toProcess[randomIndex];

            if(node is JunctionNode junctionNode)
            {
                ProcessJunctionNode(junctionNode, toProcess);
            }

            toProcess.RemoveAt(randomIndex);
        }
        if(nodes.Count > settings.MaxNodes)
        {
            Debug.Log("Max nodes reached");
        }

        Debug.Log("Fixing Graph");

        for(int i = 0; i< nodes.Count; i++)
        {
            Node node = nodes[i];

            if(node is JunctionNode junctionNode)
            {
                if(junctionNode.edges.Count == 1)
                {
                    EndNode endNode = new EndNode(node.polar);
                    ReplaceNode(junctionNode, endNode);
                }
            }
        }

        Debug.Log("Graph Generated");
    }

    bool AddNode(Node node, Node parent)
    {
        if (IsToClose(node.polar))
        {
            return false;
        }

        Edge edge = new Edge(node, parent);
        if (IsEdgeOverlapping(edge))
        {
            return false;
        }

        nodes.Add(node);
        edges.Add(edge);
        parent.AddEdge(node);
        node.AddEdge(parent);
        return true;
    }
    JunctionNode AddJunctionNode(Polar polar, Node parent, bool allowMakeConnections)
    {
        JunctionNode node = new (polar, settings, allowMakeConnections);
        if (AddNode(node, parent))
        {
            return node;
        }
        return null;
    }

    EndNode AddEndNode(Polar polar, Node parent)
    {
        EndNode node = new(polar);
        if (AddNode(node, parent))
        {
            return node;
        }

        return null;
    }
    void ReplaceNode(Node prev, Node next)
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

    void ProcessJunctionNode(JunctionNode node, List<Node> toProcess)
    {
        Polar[] polars = node.GetNext();

        for (int i = 0; i < polars.Length; i++)
        {
            Polar polar = node.polar + polars[i];
            if (IsToClose(polar))
            {
                continue;
            }

            if (Random.value < settings.EndNodeSpawnRate)
            {
                AddEndNode(polar, node);
            }
            else
            {
                JunctionNode newNode = AddJunctionNode(polar, node, true);
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
        Node[] closeNodes = GetCloseNodes(node.polar, settings.JunctionConnectionSearchDistance);
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

            Edge edge = new Edge(node, other);
            if (HasEdge(edge))
            {
                continue;
            }
            if (IsEdgeOverlapping(edge))
            {
                continue;
            }

            edges.Add(edge);
            node.AddEdge(other);
            other.AddEdge(node);
        }
    }

    bool IsEdgeOverlapping(Edge edge)
    {
        Node u = edge.a;
        Node v = edge.b;
        Vector2 p1 = u.polar.ToCartesian();
        Vector2 q1 = v.polar.ToCartesian();

        for(int i = 0; i< edges.Count; i++)
        {
            Node a = edges[i].a;
            Node b = edges[i].b;
        
            if((a == u && b == v) || (a == v && b == u))
            {
                continue;
            }

            if (a == u || a == v || b == u || b == v)
            {
                continue;
            }

            Vector2 p2 = a.polar.ToCartesian();
            Vector2 q2 = b.polar.ToCartesian();

            if(SegmentsIntersect(p1, q1, p2, q2))
            {
                return true;
            }
        }

        return false;
    }

    int Orientation(Vector2 p, Vector2 q, Vector2 r)
    {
        float val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);
        if (val == 0)
        {
            return 0;
        }

        return val > 0 ? 1 : 2;
    }   
    bool OnSegment(Vector2 p, Vector2 q, Vector2 r)
    {
        return (
            q.x <= Mathf.Max(p.x, r.x) &&
            q.x >= Mathf.Min(p.x, r.x) &&
            q.y <= Mathf.Max(p.y, r.y) &&
            q.y >= Mathf.Min(p.y, r.y)
            );
    }
    bool SegmentsIntersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
    {
        int o1 = Orientation(p1, q1, p2);
        int o2 = Orientation(p1, q1, q2);
        int o3 = Orientation(p2, q2, p1);
        int o4 = Orientation(p2, q2, q1);

        if(o1 != o2 && o3 != o4)
        {
            return true;
        }

        if (o1 == 0 && OnSegment(p1, p2, q1)) return true;
        if (o2 == 0 && OnSegment(p1, q2, q1)) return true;
        if (o3 == 0 && OnSegment(p2, p1, q2)) return true;
        if (o4 == 0 && OnSegment(p2, q1, q2)) return true;

        return false;
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

    bool IsToClose(Polar polar)
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

    Node[] GetCloseNodes(Polar polar, float testDist)
    {
        List<Node> closeNodes = new List<Node>();

        Vector2 test = polar.ToCartesian();
        for(int i = 0; i < nodes.Count; i++)
        {
            Vector2 pos = nodes[i].polar.ToCartesian();
            float dist = Vector2.Distance(pos, test);
            if(dist < testDist)
            {
                closeNodes.Add(nodes[i]);
            }
        }

        return closeNodes.ToArray();
    }
}
