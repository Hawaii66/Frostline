using UnityEngine;

namespace Frostline.World.Heights
{
    [CreateAssetMenu(menuName = "HeightSettings")]
    public class HeightSettings : ScriptableObject
    {
        [Header("Noise")]
        public float Amplitude;
        public float Frequency;
        public int Octaves;
        public float Persistence;
        public float Lacunarity;
    }
}