using UnityEngine;

namespace Frostline.World.Generation
{
    class GraphGenerationStepWithSeed : IGraphGenerationStep
    {
        public void Execute(GraphGenerationContext context)
        {
            Random.InitState(context.Settings.Seed);
        }
    }
}