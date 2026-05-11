
namespace Frostline.World.Generation
{
    class GraphGenerationStepRemoveEmptyJunctions : IGraphGenerationStep
    {
        public void Execute(GraphGenerationContext context)
        {
            for (int i = 0; i < context.Graph.Nodes.Count; i++)
            {
                Node node = context.Graph.Nodes[i];

                if (node is JunctionNode junctionNode)
                {
                    if (junctionNode.ConnectedNodes() == 1)
                    {
                        EndNode endNode = new(node.Polar);
                        context.Graph.ReplaceNode(junctionNode, endNode);
                    }
                }
            }
        }
    }
}
