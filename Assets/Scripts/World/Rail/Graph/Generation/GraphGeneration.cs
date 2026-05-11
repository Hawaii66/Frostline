using System.Collections.Generic;

namespace Frostline.World.Generation
{
    public interface IGraphGenerationStep
    {
        void Execute(GraphGenerationContext context);
    }

    public class GraphGeneration
    {
        private readonly GraphGenerationContext _context;
        private readonly List<IGraphGenerationStep> _steps;

        public GraphGeneration(GraphSettings settings)
        {
            Graph graph = new(settings);
            _context = new(graph, settings);
            _steps = new();
        }

        public GraphGeneration AddStep(IGraphGenerationStep step)
        {
            _steps.Add(step);
            return this;
        }

        public Graph Generate()
        {
            for (int i = 0; i < _steps.Count; i++)
            {
                _steps[i].Execute(_context);
            }

            return _context.Graph;
        }
    }
}