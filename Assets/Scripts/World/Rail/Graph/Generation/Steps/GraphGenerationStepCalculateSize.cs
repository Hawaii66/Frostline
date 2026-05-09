
using UnityEngine;

class GraphGenerationStepCalculateSize : GraphGenerationStep
{
    public void Execute(GraphGenerationContext context)
    {
        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;
        for (int i = 0; i < context.graph.nodes.Count; i++)
        {
            Vector2 position = context.graph.nodes[i].polar.ToCartesian();
            if(position.x < minX)
            {
                minX = Mathf.FloorToInt(position.x);
            }
            if(position.y < minY) 
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

        minX -= context.settings.BufferFromRail;
        minY -= context.settings.BufferFromRail;
        maxX += context.settings.BufferFromRail;
        maxY += context.settings.BufferFromRail;

        int sizeX = maxX - minX;
        int sizeY = maxY - minY;
        Vector2Int negativeOffset = new Vector2Int(minX, minY);
        context.graph.SizeX = sizeX;
        context.graph.SizeY = sizeY;
        context.graph.NegativeOffset = negativeOffset;
    }
}
