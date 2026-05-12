namespace Frostline.World.Tiles
{
    public interface IHeightScale
    {
        float Scale(int x, int y);
    }

    public class TileManager
    {
        private int _sizeX;
        private int _sizeY;
        private readonly TileSettings _tileSettings;

        public Tile[,] Tiles { get; private set; }

        public TileManager(TileSettings settings)
        {
            _tileSettings = settings;
        }

        public void GenerateTiles(int sizeX, int sizeY)
        {
            _sizeX = sizeX;
            _sizeY = sizeY;
            TileGeneration tileGeneration = new(_tileSettings, sizeX, sizeY);
            Tiles = tileGeneration.GenerateTiles();
        }

        public void ScaleHeights(IHeightScale scaler)
        {
            for (int x = 0; x < _sizeX; x++)
            {
                for (int y = 0; y < _sizeY; y++)
                {
                    float height = Tiles[x, y].Height;
                    Tiles[x, y].SetHeight(height * (1 - scaler.Scale(x, y)));
                }
            }
        }
    }
}
