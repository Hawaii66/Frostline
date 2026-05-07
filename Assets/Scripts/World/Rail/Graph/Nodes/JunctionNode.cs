
using System.Collections.Generic;
using UnityEngine;

public class JunctionNode : Node
{
    public GraphSettings settings;

    int junctions;
    bool allowMakeConnections;
    
    public JunctionNode(Polar polar, GraphSettings settings, bool allowMakeConnections) : base(polar)
    {
        this.settings = settings;
        this.allowMakeConnections = allowMakeConnections;
    
        junctions = settings.WithVariation(settings.JunctionCount, settings.JunctionCountVariation);
    }

    public override bool AllowsJunctionConnections()
    {
        if (!allowMakeConnections)
        {
            return false;
        }

        if (ConnectedNodes() < junctions)
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
        if (junctions == 1)
        {
            return new Polar[0];
        }

        float radSplit = Polar.CircleRad / junctions;
        List<Polar> result = new ();

        for (int i = 0; i < junctions - 1; i++)
        {
            float radVariation = radSplit * settings.WithVariation(0, settings.JunctionRadVariation);
            float radCorrection = -(Polar.CircleRad / 2 - polar.rad - radSplit);
            float rad = radSplit * i + radCorrection + radVariation;
            float dist = settings.WithVariation(settings.NodeDistance, settings.NodeDistanceVariation);

            if (Random.value < settings.NextNodeSurvivalChance)
            {
                result.Add(new Polar(rad, dist));
            }
        }

        return result.ToArray();
    }
}