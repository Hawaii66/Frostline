
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

        void GenerateStructure(Transform parent)
        {
            List<Vector2Int> positions = new();

            int childCount = parent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = parent.GetChild(i);
                positions.Add(new Vector2Int(Mathf.RoundToInt(child.position.x), Mathf.RoundToInt(child.position.z)));
            }

            StructureBlueprint sb = ScriptableObject.CreateInstance<StructureBlueprint>();
            sb.OccupiedOffsets = positions.ToArray();

            AssetDatabase.CreateAsset(sb, "Assets/Frostline/Structures/" + parent.name + ".asset");
        }
    }
}