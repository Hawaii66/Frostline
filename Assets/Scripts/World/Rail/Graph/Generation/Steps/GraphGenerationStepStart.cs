using Frostline.Core;

namespace Frostline.World.Generation
{
    class GraphGenerationStepStart : IGraphGenerationStep
    {
        public void Execute(GraphGenerationContext context)
        {
            StartNode startNode = context.Graph.TryAddStartNode(Polar.Zero);

            int startConnections = context.Settings.StartConnections;
            float radSplit = Polar.CircleRad / startConnections;
            for (int i = 0; i < startConnections; i++)
            {
                float rad = radSplit * i + radSplit * context.Settings.WithVariation(0, context.Settings.StartRadVariation);
                float dist = context.Settings.WithVariation(context.Settings.Seed, context.Settings.StartDistanceVariation);

                JunctionNode node = context.Graph.TryAddJunctionNode(new Polar(rad, dist), startNode, false);
                if (node != null)
                {
                    context.ToProcess.Add(node);
                }
            }
        }
    }
}