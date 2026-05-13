
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Frostline.World.Structures.Editor
{
    public class StructureBlueprintGenerator : MonoBehaviour
    {
        [Button("Generate Assets")]
        void Generate()
        {
            StructureBlueprintMetadata[] structures = transform.GetComponentsInChildren<StructureBlueprintMetadata>();

            HashSet<string> names = new HashSet<string>();
            for (int i = 0; i < structures.Length; i++)
            {
                if (names.Contains(structures[i].Name))
                {
                    Debug.LogWarning($"Name {structures[i].Name} is duplicate");
                }
                names.Add(structures[i].Name);

                GenerateStructure(structures[i].transform);
            }
        }

        [Button("Add Structure")]
        void AddStructure()
        {
            GameObject temp = new();
            temp.transform.parent = transform;
            temp.name = "Structure";
            temp.AddComponent<StructureBlueprintMetadata>();
            GameObject child = new();
            child.transform.parent = temp.transform;
            child.name = "Prefab";

            Selection.SetActiveObjectWithContext(temp, null);
        }

        public static void GenerateStructure(Transform parent)
        {
            StructureBlueprintMetadata metadata = parent.GetComponent<StructureBlueprintMetadata>();

            Vector2Int[] occupiedOffsets = metadata.GetOccupiedTiles();
            string name = metadata.Name;
            parent.name = name;

            if (name.Contains(" ") || name.Contains("("))
            {
                Debug.LogWarning($"Name: {name} is not valid");
                return;
            }

            string rootPath = "Assets/Frostline/Structures";
            string folderPath = $"{rootPath}/{name}";
            string prefabPath = $"{folderPath}/{name}.prefab";
            string sbPath = $"{folderPath}/{name}_SB.asset";
            string sbtPath = $"{folderPath}/{name}_SBT.asset";

            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(rootPath, name);
            }

            GameObject prefab = Instantiate(parent.GetChild(0).gameObject);
            prefab.transform.position = Vector3.zero;

            PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
            DestroyImmediate(prefab);

            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            StructureBlueprint sb = StructureBlueprint.New(prefabAsset, name, occupiedOffsets);
            AssetDatabase.CreateAsset(sb, sbPath);

            if (parent.TryGetComponent(out StructureBlueprintMetadataTrack sbmt))
            {
                StructureBlueprintTrack sbt = StructureBlueprintTrack.New(sb, sbmt.OffsetToCenterTrackPoints(), sbmt.CenterOffset());
                AssetDatabase.CreateAsset(sbt, sbtPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}