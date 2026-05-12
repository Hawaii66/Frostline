
using Frostline.World.Tiles;
using System.Threading.Tasks;
using UnityEngine;

namespace Frostline.World.Tiles
{
    public class HeightGeneration
    {
        private readonly int _sizeX;
        private readonly int _sizeY;
        private readonly TileSettings _settings;

        public HeightGeneration(TileSettings settings, int sizeX, int sizeY)
        {
            _settings = settings;
            _sizeX = sizeX;
            _sizeY = sizeY;
        }

        public float[,] GenerateHeights()
        {
            float[,] heights = new float[_sizeX, _sizeY];
            Parallel.For(0, _sizeX, (int x) =>
            {
                for (int y = 0; y < _sizeY; y++)
                {
                    heights[x, y] = TerraceHeight(GetHeight(x, y));
                }
            });

            return heights;
        }

        public static float TerraceHeight(float height)
        {
            return Mathf.RoundToInt(height);
        }

        public static int TerraceHeightInt(float height)
        {
            return Mathf.RoundToInt(height);
        }

        private float GetHeight(int x, int y)
        {
            float sampleOffset = 0.6591591f;
            float xf = x + sampleOffset;
            float yf = y + sampleOffset;

            float height = 0f;

            float amp = _settings.Amplitude;
            float freq = _settings.Frequency;
            for (int i = 0; i < _settings.Octaves; i++)
            {
                height += (Mathf.PerlinNoise(xf * freq, yf * freq) * 2 - 1) * amp;

                amp *= _settings.Persistence;
                freq *= _settings.Lacunarity;
            }

            return height;
        }
    }
}