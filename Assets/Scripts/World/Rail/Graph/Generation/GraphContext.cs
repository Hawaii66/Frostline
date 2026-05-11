
using System.Collections.Generic;

namespace Frostline.World.Generation
{
    public class GraphGenerationContext
    {
        public Graph Graph { get; }
        public GraphSettings Settings { get; }
        public List<Node> ToProcess { get; }

        public GraphGenerationContext(Graph graph, GraphSettings settings)
        {
            Graph = graph;
            Settings = settings;
            ToProcess = new List<Node>();
        }
    }
}