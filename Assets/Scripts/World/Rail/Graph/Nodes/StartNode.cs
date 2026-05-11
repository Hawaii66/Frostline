using Frostline.Core;
using UnityEngine;

namespace Frostline.World.Generation
{
    public class StartNode : Node
    {
        public StartNode(Polar polar) : base(polar) { }
        public override Color GetColor() { return Color.white; }
        public override bool AllowsJunctionConnections()
        {
            return false;
        }
    }
}