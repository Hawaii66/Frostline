using UnityEngine;

namespace Frostline.World.Generation
{
    public class GraphGenerationStepExpand : IGraphGenerationStep
    {
        public void Execute(GraphGenerationContext context)
        {
            while (context.ToProcess.Count > 0 && context.Graph.Nodes.Count <= context.Settings.MaxNodes)
            {
                int randomIndex = Random.Range(0, context.ToProcess.Count);
                Node node = context.ToProcess[randomIndex];

                if (node is JunctionNode junctionNode)
                {
                    context.Graph.ExpandFromJunction(junctionNode, context.ToProcess);
                }

                context.ToProcess.RemoveAt(randomIndex);
            }
        }
    }
}