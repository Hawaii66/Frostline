using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    public Node a;
    public Node b;

    public Edge(Node a, Node b)
    {
        this.a = a;
        this.b = b;
    }

    public static bool IsOverlapping(List<Edge> edges, Edge edge)
    {
        Node u = edge.a;
        Node v = edge.b;
        Vector2 p1 = u.polar.ToCartesian();
        Vector2 q1 = v.polar.ToCartesian();

        for (int i = 0; i < edges.Count; i++)
        {
            Node a = edges[i].a;
            Node b = edges[i].b;

            if ((a == u && b == v) || (a == v && b == u))
            {
                continue;
            }

            if (a == u || a == v || b == u || b == v)
            {
                continue;
            }

            Vector2 p2 = a.polar.ToCartesian();
            Vector2 q2 = b.polar.ToCartesian();

            if (SegmentsIntersect(p1, q1, p2, q2))
            {
                return true;
            }
        }

        return false;
    }
    static bool SegmentsIntersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
    {
        int o1 = Orientation(p1, q1, p2);
        int o2 = Orientation(p1, q1, q2);
        int o3 = Orientation(p2, q2, p1);
        int o4 = Orientation(p2, q2, q1);

        if (o1 != o2 && o3 != o4)
        {
            return true;
        }

        if (o1 == 0 && OnSegment(p1, p2, q1)) return true;
        if (o2 == 0 && OnSegment(p1, q2, q1)) return true;
        if (o3 == 0 && OnSegment(p2, p1, q2)) return true;
        if (o4 == 0 && OnSegment(p2, q1, q2)) return true;

        return false;
    }
    static int Orientation(Vector2 p, Vector2 q, Vector2 r)
    {
        float val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);
        if (val == 0)
        {
            return 0;
        }

        return val > 0 ? 1 : 2;
    }
    static bool OnSegment(Vector2 p, Vector2 q, Vector2 r)
    {
        return (
            q.x <= Mathf.Max(p.x, r.x) &&
            q.x >= Mathf.Min(p.x, r.x) &&
            q.y <= Mathf.Max(p.y, r.y) &&
            q.y >= Mathf.Min(p.y, r.y)
            );
    }
}