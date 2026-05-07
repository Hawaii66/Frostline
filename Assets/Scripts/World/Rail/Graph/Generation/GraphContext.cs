
using System.Collections.Generic;

class GraphGenerationContext
{
    public Graph graph;
    public GraphSettings settings;
    public List<Node> toProcess;

    public GraphGenerationContext(Graph graph, GraphSettings settings)
    {
        this.graph = graph;
        this.settings = settings;
        toProcess = new List<Node>();
    }
}