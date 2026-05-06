using System.Collections.Generic;
using UnityEngine;

struct ToProcess
{
    public Node parent;
    public float[][] polars;
}

class GraphGeneration
{
    Node[] nodes;
    Edge[] edges;

    public GraphGeneration(int secondNodes)
    {
        Node startNode = new(null, Node.Type.Start, 0, 0);

        float baseDistance = 20f;
        float variationDistance = 8f;

        float baseRad = Mathf.PI * 2f / secondNodes;
        float variationRad = baseRad * 0.4f;

        List<Node> nodes = new();
        List<Edge> edges = new();

        nodes.Add(startNode);


        List<ToProcess> toProcess = new List<ToProcess>();
        for (int i = 0; i < secondNodes; i++)
        {
            float rad = baseRad * i + Random.Range(-variationRad / 2, variationRad / 2);
            float dist = baseDistance + Random.Range(-variationDistance / 2, variationDistance / 2);
            JunctionNode node = new(startNode, dist, rad, 4);
            nodes.Add(node);
            edges.Add(new(startNode, node));

            float[][] polars = node.NextPolarCoordinates();
            toProcess.Add(new ToProcess { 
                parent = node,
                polars = polars,
            });
        }

        while(toProcess.Count > 0)
        {
            ToProcess process = toProcess[0];
            for(int j = 0; j < process.polars.Length; j++)
            {
                float dist = process.polars[j][0];
                float rad = process.polars[j][1];

                bool isJunction = Random.value < 0.3f;
                Node node;
                if (isJunction)
                {
                    JunctionNode junctionNode = new JunctionNode(process.parent, dist, rad, Random.Range(2,5));
                    float[][] polars = junctionNode.NextPolarCoordinates();
                    toProcess.Add(new ToProcess
                    {
                        parent = junctionNode,
                        polars = polars,
                    });
                    node = junctionNode;
                }
                else
                {
                    node = new(process.parent, Node.Type.End, process.polars[j][0], process.polars[j][1]);
                }

                nodes.Add(node);
                edges.Add(new(process.parent, node));
            }

            toProcess.RemoveAt(0);
        }

        this.nodes = nodes.ToArray();
        this.edges = edges.ToArray();
    }

    public void DEBUG_Log()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            Node node = nodes[i];
            Vector2 pos = node.GetPosition();
            Debug.Log("Node: x:" + pos.x + " y: " + pos.y);
        }
    }

    public Vector2[] DEBUG_GetNodePositions()
    {
        Vector2[] positions = new Vector2[nodes.Length];

        for (int i = 0; i < nodes.Length; i++)
        {
            positions[i] = nodes[i].GetPosition();
        }

        return positions;
    }

    public Vector2[][] DEBUG_GetEdgePositions()
    {
        Vector2[][] positions = new Vector2[edges.Length][];

        for (int i = 0; i < edges.Length; i++) {
            Node start = edges[i].GetStartNode();
            Node end = edges[i].GetEndNode();

            positions[i] = new Vector2[] { start.GetPosition(), end.GetPosition() };
        }

        return positions;
    }
}

class JunctionNode : Node
{
    int junctions;
    public JunctionNode(Node parent, float distance, float rad, int junctions) : base(parent, Type.Junction, distance, rad)
    {
        this.junctions = junctions;
    }

    public float[][] NextPolarCoordinates()
    {
        float[][] polars = new float[junctions - 1][];

        for(int i = 0; i < junctions - 1; i++)
        {
            float split = Mathf.PI * 2 / junctions;
            float newRad = rad + split * (i + junctions - 1);
            float variationRad = split * 0.0f;
            newRad += variationRad;
            float newDist = 3f + Random.Range(2f, 5f);

            polars[i] = new float[] { newDist, newRad };
        }

        return polars;
    }
}

class Node
{
    public enum Type
    {
        Start,
        End,
        Junction,
        Reward
    }
    Type type;
    float distance;
    public float rad;
    Node parent;

    public Node(Node parent, Type type, float distance, float rad)
    {
        this.distance = distance;
        this.rad = rad;
        this.parent = parent;
        this.type = type;
    }

    public Vector2 GetPosition()
    {
        Vector2 position = PolarToCartesian();
        if (parent != null)
        {
            position += parent.GetPosition();
        }

        return position;
    }

    public int NodesToStart()
    {
        if(parent == null)
        {
            return 0;
        }

        return parent.NodesToStart() + 1;
    }

    Vector2 PolarToCartesian()
    {
        float x = Mathf.Cos(rad) * distance;
        float y = Mathf.Sin(rad) * distance;

        return new Vector2(x, y);
    }

}

class Edge
{
    Node start;
    Node end;

    public Edge(Node start, Node end)
    {
        this.start = start;
        this.end = end;
    }

    public Node GetStartNode()
    {
        return start;
    }
    public Node GetEndNode() {
        return end;
    }
                
}