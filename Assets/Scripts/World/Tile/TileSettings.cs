using UnityEngine;

namespace Frostline.World.Tiles
{
    [CreateAssetMenu(menuName = "TileSettings")]
    public class TileSettings : ScriptableObject
    {
        [Header("Tile Settings")]
        public Color[] TileColors;

        [Header("Height Settings")]
        public float Amplitude;
        public float Frequency;
        public int Octaves;
        public float Persistence;
        public float Lacunarity;

        public Color RandomColor()
        {
            return TileColors[Random.Range(0, TileColors.Length)];
        }
    }
}