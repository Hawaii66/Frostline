using UnityEngine;

class GraphGenerationStepWithSeed : GraphGenerationStep
{
    public void Execute(GraphGenerationContext context)
    {
        Random.InitState(context.settings.seed);
    }
}
