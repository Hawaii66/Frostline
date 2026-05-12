using UnityEngine;

namespace Frostline.World.Tiles
{
    public class Tile
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Height { get; private set; }
        public Color Color { get; private set; }

        public Tile(int x, int y, int height, Color color)
        {
            X = x;
            Y = y;
            Height = height;
            Color = color;
        }

        public void SetHeight(int height)
        {
            Height = height;
        }
    }
}
