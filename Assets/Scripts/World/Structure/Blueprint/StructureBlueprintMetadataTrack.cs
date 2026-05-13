using UnityEditor;
using UnityEngine;

namespace Frostline.World.Structures
{
    public class StructureBlueprintMetadataTrack : MonoBehaviour
    {
        public Vector2Int[] TrackPoints;

        private void OnDrawGizmos()
        {
            if (TrackPoints == null)
            {
                return;

            }

            Gizmos.color = Color.orangeRed;
            for (int i = 0; i < TrackPoints.Length - 1; i++)
            {
                Vector3 start = new(TrackPoints[i].x, 0, TrackPoints[i].y);
                Vector3 end = new(TrackPoints[i + 1].x, 0, TrackPoints[i + 1].y);
                Gizmos.DrawLine(start + transform.position, end + transform.position);
            }


            //if (TrackPoints.Length >= 2)
            //{
            //  Vector2 toTest = Vector2.up;
            //  Vector2 end = TrackPoints[TrackPoints.Length - 1] - TrackPoints[TrackPoints.Length - 2];
            //  Vector2 start = TrackPoints[1] - TrackPoints[0];
            //  Handles.Label(transform.position + Vector3.up, $"{Vector2.Dot(start.normalized, toTest.normalized)} {Vector2.Dot(end.normalized, toTest.normalized)}");
            //}
        }

        public Vector2Int[] OffsetToCenterTrackPoints()
        {
            Vector2Int offset = TrackPoints[0];

            Vector2Int[] offsets = new Vector2Int[TrackPoints.Length];
            for (int i = 0; i < TrackPoints.Length; i++)
            {
                offsets[i] = TrackPoints[i] - offset;
            }

            return offsets;
        }
        public Vector2Int CenterOffset()
        {
            return TrackPoints[0];
        }
    }
}