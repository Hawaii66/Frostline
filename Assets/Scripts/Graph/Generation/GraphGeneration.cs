using System.Collections.Generic;

class GraphGeneration
{
    Graph graph;
    GraphGenerationContext context;
    List<GraphGenerationStep> steps;

    public GraphGeneration(GraphSettings settings)
    {
        graph = new (settings);
        context = new(graph, settings);
        steps = new();
    }

    public GraphGeneration AddStep(GraphGenerationStep step)
    {
        steps.Add(step);
        return this;
    }

    public Graph Generate()
    {
        for(int i = 0; i < steps.Count; i++)
        {
            steps[i].Execute(context);
        }

        return graph;
    }
}


interface GraphGenerationStep
{
    void Execute(GraphGenerationContext context);
}