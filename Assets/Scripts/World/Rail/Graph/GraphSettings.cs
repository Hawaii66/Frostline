using UnityEngine;

namespace Frostline.World.Generation
{
    [CreateAssetMenu(menuName = "GraphSettings")]
    public class GraphSettings : ScriptableObject
    {
        [Header("General")]
        public int Seed;
        public int MaxNodes;
        [Range(3, 10)]
        public int StartConnections;
        [Range(0f, 0.5f)]
        public float StartRadVariation;
        [Range(20, 100)]
        public int BufferFromRail;

        [Header("Junctions")]
        [Range(3, 10)]
        public int JunctionCount;
        [Range(0, 5)]
        public int JunctionCountVariation;
        [Range(0, 0.5f)]
        public float JunctionRadVariation;
        [Range(0f, 1f)]
        public float NextNodeSurvivalChance;
        [Range(0f, 1f)]
        public float JunctionConnectionChance;

        [Header("Spawn Rate")]
        [Range(0f, 1f)]
        public float EndNodeSpawnRate;
        [Range(0f, 1f)]
        public float JunctionChallengeRate;

        [Header("Size")]
        public float StartDistance;
        public float StartDistanceVariation;
        public float NodeDistance;
        public float NodeDistanceVariation;
        public float MinDistanceBetweenNodes;
        public float JunctionConnectionSearchDistance;

        public float WithVariation(float val, float variation)
        {
            float var = -variation + Random.value * (variation * 2);
            return val + var;
        }
        public int WithVariation(int val, int variation)
        {
            return Random.Range(val - variation, val + variation + 1);
        }

    }
}