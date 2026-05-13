
using UnityEngine;

namespace Frostline.World.Structures
{
    public class StructureBlueprintTrack : ScriptableObject
    {
        public StructureBlueprint StructureBlueprint;
        public Vector2Int[] TrackSegments;
        public Vector2Int CenterOffset;

        public static StructureBlueprintTrack New(StructureBlueprint sb, Vector2Int[] track, Vector2Int centerOffset)
        {
            StructureBlueprintTrack sbt = CreateInstance<StructureBlueprintTrack>();
            sbt.StructureBlueprint = sb;
            sbt.TrackSegments = track;
            sbt.CenterOffset = centerOffset;
            return sbt;
        }
    }
}