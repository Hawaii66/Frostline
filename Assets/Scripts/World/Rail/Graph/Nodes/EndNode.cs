using Frostline.Core;
using UnityEngine;

namespace Frostline.World.Generation
{
    public class EndNode : Node
    {
        public EndNode(Polar polar) : base(polar) { }
        public override Color GetColor() { return Color.blanchedAlmond; }
        public override bool AllowsJunctionConnections()
        {
            return false;
        }
    }
}