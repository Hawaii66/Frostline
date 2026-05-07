using System.Diagnostics;
using UnityEngine;

public class GraphGenerationStepExpand : GraphGenerationStep
{
    void GraphGenerationStep.Execute(GraphGenerationContext context)
    {
        while (context.toProcess.Count > 0 && context.graph.nodes.Count <= context.settings.MaxNodes)
        {
            int randomIndex = Random.Range(0, context.toProcess.Count);
            Node node = context.toProcess[randomIndex];

            if (node is JunctionNode junctionNode)
            {
                context.graph.ExpandFromJunction(junctionNode, context.toProcess);
            }

            context.toProcess.RemoveAt(randomIndex);
        }
    }
}