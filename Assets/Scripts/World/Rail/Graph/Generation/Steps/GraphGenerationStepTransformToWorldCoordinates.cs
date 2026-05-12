using Frostline.Core;
using UnityEngine;

namespace Frostline.World.Generation
{
    public class GraphGenerationStepTransformToWorldCoordinates : IGraphGenerationStep
    {
        public void Execute(GraphGenerationContext context)
        {
            Vector2Int negativeOffset = context.Graph.NegativeOffset;
            int worldScale = context.Settings.WorldScale;
            for (int i = 0; i < context.Graph.Nodes.Count; i++)
            {
                Polar polar = context.Graph.Nodes[i].Polar;
                Vector2 worldPosition = (polar.ToCartesian() - negativeOffset) * worldScale;
                context.Graph.Nodes[i].SetPosition(worldPosition);
            }

            int sixeX = context.Graph.SizeX * worldScale;
            int sizeY = context.Graph.SizeY * worldScale;
            context.Graph.SetSize(sixeX, sizeY, negativeOffset * worldScale);
        }
    }
}