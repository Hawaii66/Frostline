
class GraphGenerationStepRemoveEmptyJunctions : GraphGenerationStep
{
    public void Execute(GraphGenerationContext context)
    {
        for (int i = 0; i < context.graph.nodes.Count; i++)
        {
            Node node = context.graph.nodes[i];

            if (node is JunctionNode junctionNode)
            {
                if (junctionNode.ConnectedNodes() == 1)
                {
                    EndNode endNode = new(node.polar);
                    context.graph.ReplaceNode(junctionNode, endNode);
                }
            }
        }    }
}
