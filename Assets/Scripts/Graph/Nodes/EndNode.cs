using UnityEngine;

public class EndNode : Node
{
    public EndNode(Polar polar) : base(polar) { }
    public override Color GetColor() { return Color.blanchedAlmond; }
    public override bool AllowsJunctionConnections()
    {
        return false;
    }
}