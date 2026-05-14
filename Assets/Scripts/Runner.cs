using Frostline.Core;
using Frostline.World;
using Frostline.World.Structures;
using Frostline.World.Tracks;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Frostline.DEBUG
{
    public class Runner : MonoBehaviour, IRequireServices
    {
        StructureManager world;
        Dictionary<string, StructureBlueprint> structureBlueprints;

        [Button("Generate world")]
        void Run3()
        {
            world = new StructureManager();
            StructureBlueprint structureBlueprint = StructureBlueprint.New(null, "", new Vector2Int[]
            {
            new (0,0),
            new (1,0),
            new (1,1),
            new (0,1),
            }, new Vector2Int[0]);
            Structure structure = new(structureBlueprint, Vector2Int.zero);
            world.TryAddStructure(structure);

            Structure structure2 = new(structureBlueprint, Vector2Int.right);
            world.TryAddStructure(structure2);

            StructureBlueprint structureBlueprint2 = StructureBlueprint.New(null, "", new Vector2Int[]
            {
            new (0,0),
            new (1,0),
            new (1,1),
            new (0,1),
            }, new Vector2Int[0]);
            Structure structure3 = new(structureBlueprint2, Vector2Int.right * 3 + Vector2Int.up * 2);
            world.TryAddStructure(structure3);

            if (structureBlueprints.TryGetValue("Cube", out StructureBlueprint sb))
            {
                world.TryAddStructure(new(sb, new Vector2Int(20, 20)));
            }
        }

        private void OnDrawGizmos()
        {
            if (world != null)
            {
                Gizmos.color = Color.black;
                for (int i = 0; i < world.Structures.Count; i++)
                {
                    Structure structure = world.Structures[i];
                    for (int j = 0; j < structure.GetOccupiedPositions().Length; j++)
                    {
                        Vector2Int position = structure.GetOccupiedPositions()[j];
                        Gizmos.DrawWireSphere(new Vector3(position.x, 0, position.y), 0.2f);
                    }
                }
            }

            if (_tsm != null && _tsm.TrackPaths != null)
            {
                Gizmos.color = Color.yellow;

                List<List<Vector2Int>> trackPaths = _tsm.TrackPaths;
                for (int i = 0; i < trackPaths.Count; i++)
                {
                    List<Vector2Int> track = trackPaths[i];
                    bool hasPath = _tsm.HasTrack(track[0], track[track.Count - 1]);
                    if (hasPath)
                    {
                        Gizmos.color = Color.green;
                    }
                    else
                    {
                        Gizmos.color = Color.yellow;
                    }
                    for (int j = 0; j < track.Count - 1; j++)
                    {
                        Vector3 start = new(track[j].x, 0, track[j].y);
                        Vector3 end = new(track[j + 1].x, 0, track[j + 1].y);
                        Gizmos.DrawLine(start, end);
                    }
                }
            }
        }

        private TrackSegmentManager _tsm;

        public void Initialize(IServiceRegistry serviceRegistry)
        {
            _tsm = serviceRegistry.GetService<TrackSegmentManager>();
        }
    }
}