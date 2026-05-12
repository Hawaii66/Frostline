using UnityEngine;

namespace Frostline.World.Tiles
{
    public class Tile
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public float Height { get; private set; }
        public int HeightInt => HeightGeneration.TerraceHeightInt(Height);
        public Color Color { get; private set; }

        public Tile(int x, int y, float height, Color color)
        {
            X = x;
            Y = y;
            Height = height;
            Color = color;
        }

        public void SetHeight(float height)
        {
            Height = HeightGeneration.TerraceHeight(height);
        }
    }
}
