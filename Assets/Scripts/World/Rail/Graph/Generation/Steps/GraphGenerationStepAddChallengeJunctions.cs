using UnityEngine;
namespace Frostline.World.Generation
{
    class GraphGenerationStepAddChallengeJunctions : IGraphGenerationStep
    {
        public void Execute(GraphGenerationContext context)
        {
            for (int i = 0; i < context.Graph.Nodes.Count; i++)
            {
                Node node = context.Graph.Nodes[i];

                if (node is JunctionNode junctionNode)
                {
                    bool isPassThrough = junctionNode.ConnectedNodes() == 2;
                    float challengeRate = context.Settings.JunctionChallengeRate * (isPassThrough ? 1.5f : 1);
                    if (!(Random.value < challengeRate))
                    {
                        continue;
                    }

                    ChallengeJunction challengeJunctionNode = new(node.Polar, context.Settings, true);
                    context.Graph.ReplaceNode(junctionNode, challengeJunctionNode);
                }
            }
        }
    }
}
