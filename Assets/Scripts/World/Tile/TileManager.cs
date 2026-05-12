using Frostline.Core;
using System.Threading.Tasks;
using UnityEngine;

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

        private Tile[,] _tiles;

        public void GenerateTiles(int sizeX, int sizeY)
        {
            _sizeX = sizeX;
            _sizeY = sizeY;
            TileGeneration tileGeneration = new(_tileSettings, sizeX, sizeY);
            _tiles = tileGeneration.GenerateTiles();
        }

        public void ScaleHeights(IHeightScale scaler)
        {
            Parallel.For(0, _sizeX, (int x) =>
            {
                for (int y = 0; y < _sizeY; y++)
                {
                    int height = _tiles[x, y].Height;
                    float scaledHeight = height * (1 - scaler.Scale(x, y));
                    _tiles[x, y].SetHeight(HeightGeneration.TerraceHeight(scaledHeight));
                }
            });
        }

        public void Initialize(IServiceRegistry serviceRegistry)
        {
            _tileSettings = serviceRegistry.GetService<WorldSettings>().TileSettings;
        }

        public Tile GetTile(int x, int y)
        {
            return _tiles[x, y];
        }
        public Tile GetTile(Vector2Int pos)
        {
            return GetTile(pos.x, pos.y);
        }
    }
}
