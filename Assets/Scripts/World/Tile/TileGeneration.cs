namespace Frostline.World.Tiles
{
    public class TileGeneration
    {
        private readonly int _sizeX;
        private readonly int _sizeY;
        private readonly TileSettings _tileSettings;
        private readonly HeightGeneration _heightGeneration;

        public TileGeneration(TileSettings tileSettings, int sizeX, int sizeY)
        {
            _sizeX = sizeX;
            _sizeY = sizeY;
            _tileSettings = tileSettings;
            _heightGeneration = new HeightGeneration(tileSettings, sizeX, sizeY);
        }

        public Tile[,] GenerateTiles()
        {
            int[,] heights = _heightGeneration.GenerateHeights();

            Tile[,] tiles = new Tile[_sizeX, _sizeY];
            for (int x = 0; x < _sizeX; x++)
            {
                for (int y = 0; y < _sizeY; y++)
                {
                    Tile tile = new Tile(x, y, heights[x, y], _tileSettings.RandomColor());
                    tiles[x, y] = tile;
                }
            }

            return tiles;
        }
    }
}