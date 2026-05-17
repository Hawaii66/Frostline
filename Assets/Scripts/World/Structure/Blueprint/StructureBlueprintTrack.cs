using UnityEngine;

namespace Frostline.World.Structures
{
    [System.Serializable]
    public struct TrackPath
    {
        public Vector2Int[] Path;
        public Vector2Int CenterOffset;
    }

    public class StructureBlueprintTrack : ScriptableObject
    {
        public StructureBlueprint StructureBlueprint;
        public TrackPath[] TrackPaths;

        public static StructureBlueprintTrack New(StructureBlueprint sb, TrackPath[] trackPaths)
        {
            StructureBlueprintTrack sbt = CreateInstance<StructureBlueprintTrack>();
            sbt.StructureBlueprint = sb;
            sbt.TrackPaths = trackPaths;
            return sbt;
        }
    }
}