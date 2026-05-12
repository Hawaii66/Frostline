using Frostline.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Frostline.World.Structures
{
    public class StructureBlueprintManager : IRequireServices
    {
        private Dictionary<string, StructureBlueprint> structureBlueprints;

        public void Initialize(IServiceRegistry serviceRegistry)
        {
            LoadStructures();
        }

        public void LoadStructures()
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

        public bool TryGetStructureBlueprint(string name, out StructureBlueprint structureBlueprint)
        {
            if (structureBlueprints.TryGetValue(name, out structureBlueprint))
            {
                return true;
            }
            return false;
        }
    }
}