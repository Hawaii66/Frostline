using UnityEngine;

public class ChallengeJunction : JunctionNode
{
    public ChallengeJunction(Polar polar, GraphSettings settings, bool allowMakeConnections) : base(polar, settings, allowMakeConnections) { }
    public override Color GetColor()
    {
        return Color.green;
    }
}