namespace Frostline.World.Heights
{
    public interface IHeightScale
    {
        float Scale(int x, int y);
    }

    public class Height
    {
        private readonly int _sizeX;
        private readonly int _sizeY;

        public float[,] Heights { get; private set; }

        public Height(HeightSettings settings, int sizeX, int sizeY)
        {
            _sizeX = sizeX;
            _sizeY = sizeY;
            HeightGeneration generation = new(settings, sizeX, sizeY);
            Heights = generation.GenerateHeights();
        }

        public void ScaleHeights(IHeightScale scaler)
        {
            for (int x = 0; x < _sizeX; x++)
            {
                for (int y = 0; y < _sizeY; y++)
                {
                    Heights[x, y] *= (1 - scaler.Scale(x, y));
                }
            }
        }
    }
}
