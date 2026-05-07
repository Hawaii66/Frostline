
class GraphGenerationStepStart : GraphGenerationStep
{
    public void Execute(GraphGenerationContext context)
    {
        StartNode startNode = new(Polar.Zero);
        context.graph.nodes.Add(startNode);

        int startConnections = context.settings.StartConnections;
        float radSplit = Polar.CircleRad / startConnections;
        for (int i = 0; i < startConnections; i++)
        {
            float rad = radSplit * i + radSplit * context.settings.WithVariation(0, context.settings.StartRadVariation);
            float dist = context.settings.WithVariation(context.settings.StartDistance, context.settings.StartDistanceVariation);

            JunctionNode node = context.graph.TryAddJunctionNode(new Polar(rad, dist), startNode, false);
            if (node != null)
            {
                context.toProcess.Add(node);
            }
        }
    }
}