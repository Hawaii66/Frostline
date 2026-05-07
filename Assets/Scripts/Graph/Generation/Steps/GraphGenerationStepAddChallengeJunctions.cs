using UnityEngine;

class GraphGenerationStepAddChallengeJunctions : GraphGenerationStep
{
    public void Execute(GraphGenerationContext context)
    {
        for (int i = 0; i < context.graph.nodes.Count; i++)
        {
            Node node = context.graph.nodes[i];

            if (node is JunctionNode junctionNode)
            {
                bool isPassThrough = junctionNode.ConnectedNodes() == 2;
                float challengeRate = context.settings.JunctionChallengeRate * (isPassThrough ? 1.5f : 1);
                if (!(Random.value < challengeRate))
                {
                    continue;
                }

                ChallengeJunction challengeJunctionNode = new(node.polar, context.settings, true);
                context.graph.ReplaceNode(junctionNode, challengeJunctionNode);
            }
        }
    }
}
