namespace Frostline.World.Generation
{

    public class GraphGenerator
    {
        public static Graph Generate(GraphSettings settings)
        {
            return new GraphGeneration(settings)
                .AddStep(new GraphGenerationStepWithSeed())
                .AddStep(new GraphGenerationStepStart())
                .AddStep(new GraphGenerationStepExpand())
                .AddStep(new GraphGenerationStepRemoveEmptyJunctions())
                .AddStep(new GraphGenerationStepAddChallengeJunctions())
                .AddStep(new GraphGenerationStepCalculateSize())
                .AddStep(new GraphGenerationStepTransformToWorldCoordinates())
                .Generate();
        }
    }
}