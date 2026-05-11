
using UnityEngine;
namespace Frostline.World.Generation
{
    class GraphGenerationStepCalculateSize : IGraphGenerationStep
    {
        public void Execute(GraphGenerationContext context)
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;
            for (int i = 0; i < context.Graph.Nodes.Count; i++)
            {
                Vector2 position = context.Graph.Nodes[i].Polar.ToCartesian();
                if (position.x < minX)
                {
                    minX = Mathf.FloorToInt(position.x);
                }
                if (position.y < minY)
                {
                    minY = Mathf.FloorToInt(position.y);
                }
                if (position.x > maxX)
                {
                    maxX = Mathf.CeilToInt(position.x);
                }
                if (position.y > maxY)
                {
                    maxY = Mathf.CeilToInt(position.y);
                }
            }

            minX -= context.Settings.BufferFromRail;
            minY -= context.Settings.BufferFromRail;
            maxX += context.Settings.BufferFromRail;
            maxY += context.Settings.BufferFromRail;

            int sizeX = maxX - minX;
            int sizeY = maxY - minY;
            Vector2Int negativeOffset = new(minX, minY);
            context.Graph.SetSize(sizeX, sizeY, negativeOffset);
        }
    }
}