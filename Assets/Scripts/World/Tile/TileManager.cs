using Frostline.Core;
using System.Threading.Tasks;

namespace Frostline.World.Tiles
{
    public interface IHeightScale
    {
        float Scale(int x, int y);
    }

    public class TileManager : IRequireServices
    {
        private int _sizeX;
        private int _sizeY;
        private TileSettings _tileSettings;

        public Tile[,] Tiles { get; private set; }

        public void GenerateTiles(int sizeX, int sizeY)
        {
            _sizeX = sizeX;
            _sizeY = sizeY;
            TileGeneration tileGeneration = new(_tileSettings, sizeX, sizeY);
            Tiles = tileGeneration.GenerateTiles();
        }

        public void ScaleHeights(IHeightScale scaler)
        {
            Parallel.For(0, _sizeX, (int x) =>
            {
                for (int y = 0; y < _sizeY; y++)
                {
                    float height = Tiles[x, y].Height;
                    Tiles[x, y].SetHeight(height * (1 - scaler.Scale(x, y)));
                }
            });
        }

        public void Initialize(IServiceRegistry serviceRegistry)
        {
            _tileSettings = serviceRegistry.GetService<WorldSettings>().TileSettings;
        }
    }
}
