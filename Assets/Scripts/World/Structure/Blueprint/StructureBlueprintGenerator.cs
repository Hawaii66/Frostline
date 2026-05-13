
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

            string rootPath = "Assets/Frostline/Structures";
            string folderPath = $"{rootPath}/{name}";
            string prefabPath = $"{folderPath}/{name}.prefab";
            string sbPath = $"{folderPath}/{name}_SB.asset";
            string sbtPath = $"{folderPath}/{name}_SBT.asset";

            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(rootPath, name);
            }

            GameObject prefab = Instantiate(parent.gameObject);
            prefab.transform.position = Vector3.zero;
            DestroyImmediate(prefab.GetComponent<StructureBlueprintMetadata>());
            DestroyImmediate(prefab.GetComponent<StructureBlueprintMetadataTrack>());

            PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
            DestroyImmediate(prefab);

            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            StructureBlueprint sb = StructureBlueprint.New(prefabAsset, occupiedOffsets);
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