
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

            for (int i = 0; i < structures.Length; i++)
            {
                GenerateStructure(structures[i].transform);
            }
        }

        public static void GenerateStructure(Transform parent)
        {
            StructureBlueprintMetadata metadata = parent.GetComponent<StructureBlueprintMetadata>();

            Vector2Int[] occupiedOffsets = metadata.GetOccupiedTiles();
            string name = parent.name;

            if (name.Contains(" ") || name.Contains("("))
            {
                Debug.LogWarning($"Name: {name} is not valid");
                return;
            }

            GameObject prefab = Instantiate(parent.gameObject);
            prefab.transform.position = Vector3.zero;
            DestroyImmediate(prefab.GetComponent<StructureBlueprintMetadata>());

            string rootPath = "Assets/Frostline/Structures";
            string folderPath = $"{rootPath}/{name}";
            string prefabPath = $"{folderPath}/{name}.prefab";
            string sbPath = $"{folderPath}/{name}.asset";

            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(rootPath, name);
            }
            PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
            DestroyImmediate(prefab);

            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            StructureBlueprint sb = StructureBlueprint.New(prefabAsset, occupiedOffsets);
            AssetDatabase.CreateAsset(sb, sbPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}