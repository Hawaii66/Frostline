using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node {
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

    public abstract Color GetColor();
    public abstract bool AllowsJunctionConnections();
}

public class StartNode : Node
{
    public StartNode(Polar polar) : base(polar) { }
    public override Color GetColor() { return Color.white; }
    public override bool AllowsJunctionConnections()
    {
        return false;
    }

}

public class EndNode : Node
{
    public EndNode(Polar polar) : base(polar) { }
    public override Color GetColor() { return Color.blanchedAlmond;  }
    public override bool AllowsJunctionConnections()
    {
        return false;
    }
}


public class JunctionNode : Node
{
    int junctions;
    bool allowMakeConnections;
    public GraphSettings settings;
    public JunctionNode(Polar polar, GraphSettings settings, bool allowMakeConnections) : base(polar)
    {
        this.settings = settings;
        junctions = settings.WithVariation(settings.JunctionCount, settings.JunctionCountVariation);
        this.allowMakeConnections = allowMakeConnections;
    }

    public override bool AllowsJunctionConnections()
    {
        if (!allowMakeConnections)
        {
            return false;
        }

        if(edges.Count < junctions)
        {
            return true;
        }

        return false;
    }


    public override Color GetColor()
    {
        return Color.yellow;
    }

    public Polar[] GetNext()
    {
        if(junctions == 1)
        {
            return new Polar[0];
        }
        
        float radSplit = Polar.CircleRad / junctions;
        List<Polar> result = new List<Polar>();

        for(int i = 0; i < junctions - 1; i++)
        {
            float rad = radSplit * i - (Polar.CircleRad / 2 - polar.rad - radSplit) + radSplit * settings.WithVariation(0,settings.JunctionRadVariation);
            float dist = settings.WithVariation(settings.NodeDistance, settings.NodeDistanceVariation);

            if(Random.value < settings.NextNodeSurvivalChance) { 
                result.Add(new Polar(rad, dist));
            }
        }

        return result.ToArray();
    }
}