using Frostline.World.Generation;
using Frostline.World.Tiles;
using UnityEngine;

namespace Frostline.World
{
    [CreateAssetMenu(menuName = "Frostline/WorldSettings")]
    public class WorldSettings : ScriptableObject
    {
        [Header("World Settings")]
        public int Seed;
        public int ViewRadius;
        public Vector2Int Size;
        public int StructureCount;
        public int StructureBorderBufferRadius;
        public int MinimumDistanceBetweenJunctions;
        public float ExtraJunctionSurvivalRate;

        [Header("Special Settings")]
        public GraphSettings GraphSettings;
        public TileSettings TileSettings;

    }
}