using Frostline.Core;
using Frostline.World.Structures;
using NaughtyAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Frostline.Test
{
    public class WorldGeneration2Debug : MonoBehaviour, IRequireServices
    {
        private WorldGeneration2 _worldGen;
        private WorldGenerationResult _result;

        [Button("Generate")]
        private void Generate()
        {
            _result = _worldGen.Generate(new Vector2Int(100, 100));
        }

        private void OnDrawGizmos()
        {
            if (_result == null) { return; }

            int sizeX = _result.Tiles.GetLength(0);
            int sizeY = _result.Tiles.GetLength(1);

            Vector3 center = new(sizeX / 2, 0, sizeY / 2);
            Vector3 size = new(sizeX, 1, sizeY);
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(center, size);

            Gizmos.color = Color.black;
            List<Structure> structures = _result.Structures;
            foreach (Structure structure in structures)
            {
                Vector2Int[] positions = structure.GetOccupiedPositions();
                foreach (Vector2Int position in positions)
                {
                    Vector3 pos = new(position.x, 0, position.y);
                    Gizmos.DrawWireSphere(pos, 0.3f);
                }
            }
        }

        public void Initialize(IServiceRegistry serviceRegistry)
        {
            _worldGen = serviceRegistry.GetService<WorldGeneration2>();
        }
    }
}