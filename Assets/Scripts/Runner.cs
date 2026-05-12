using Frostline.World;
using Frostline.World.Structures;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Frostline.DEBUG
{
    public class Runner : MonoBehaviour
    {
        WorldContext world;
        Dictionary<string, StructureBlueprint> structureBlueprints;

        [Button("Load structures")]
        void RunStructureLoad()
        {
            structureBlueprints = new();

            string[] guids = AssetDatabase.FindAssets($"t:{typeof(StructureBlueprint).Name}", new[] { "Assets/Frostline/Structures" });
            StructureBlueprint[] filtered = guids.Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<StructureBlueprint>)
                .ToArray();

            for (int i = 0; i < filtered.Length; i++)
            {
                structureBlueprints.Add(filtered[i].name, filtered[i]);
            }
        }

        [Button("Generate world")]
        void Run3()
        {
            world = new WorldContext();
            StructureBlueprint structureBlueprint = StructureBlueprint.New(new Vector2Int[]
            {
            new (0,0),
            new (1,0),
            new (1,1),
            new (0,1),
            });
            Structure structure = new(structureBlueprint, Vector2Int.zero);
            world.TryAddStructure(structure);

            Structure structure2 = new(structureBlueprint, Vector2Int.right);
            world.TryAddStructure(structure2);

            StructureBlueprint structureBlueprint2 = StructureBlueprint.New(new Vector2Int[]
            {
            new (0,0),
            new (1,0),
            new (1,1),
            new (0,1),
            });
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
        }
    }
}