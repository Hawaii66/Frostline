using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[CreateAssetMenu(menuName ="GraphSettings")]
public class GraphSettings : ScriptableObject
{
    [Header("General")]
    public int seed;
    public int MaxNodes;
    [Range(3, 10)]
    public int StartConnections;
    [Range(0f, 0.5f)]
    public float StartRadVariation;

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
    [Range(0f,1f)]
    public float EndNodeSpawnRate;

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
        float variationf = variation;

        int var = Mathf.RoundToInt(-variationf + Random.value * (variationf * 2));
        return val + var;
    }

}