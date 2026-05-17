using Frostline.Core;
using Frostline.World.Structures;
using NaughtyAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
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
            _result = _worldGen.Generate(new Vector2Int(500, 500));
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

            List<Structure> structures = _result.Structures;
            foreach (Structure structure in structures)
            {
                Gizmos.color = Color.black;
                Vector2Int[] positions = structure.GetOccupiedPositions();
                foreach (Vector2Int position in positions)
                {
                    Vector3 pos = new(position.x, 0, position.y);
                    Gizmos.DrawWireSphere(pos, 0.4f);
                }

                Gizmos.color = Color.coral;
                Vector2Int[] bounds = structure.GetBounds();
                Vector2Int boundLowerLeft = bounds[0];
                Vector2Int boundUpperRight = bounds[bounds.Length - 1];
                Vector3 boundCenter = new((boundLowerLeft.x + boundUpperRight.x) / 2, 0, (boundLowerLeft.y + boundUpperRight.y) / 2);
                Vector3 boundSize = new(boundUpperRight.x - boundLowerLeft.x, 0.2f, boundUpperRight.y - boundLowerLeft.y);
                Gizmos.DrawWireCube(boundCenter, boundSize);
            }

            Gizmos.color = Color.wheat;
            foreach (Vector2Int position in _result.TrackNodes)
            {
                Vector3 pos = new(position.x, 0, position.y);
                Gizmos.DrawWireSphere(pos, 0.2f);
            }

            Gizmos.color = Color.darkSlateBlue;
            foreach (DebugText debugText in _result.DebugTexts)
            {
                Vector3 pos = new(debugText.Position.x, 0, debugText.Position.y);
                Handles.Label(pos, debugText.Text);
            }

            Gizmos.color = Color.blanchedAlmond;
            for (int i = 0; i < _result.TrackEdges.Count; i++)
            {
                (Vector2Int start, Vector2Int end) = _result.TrackEdges[i];
                Vector3 s = new(start.x, 0, start.y);
                Vector3 e = new(end.x, 0, end.y);
                Gizmos.DrawLine(s, e);
            }

            Gizmos.color = Color.blueViolet;
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if (_result.JunctionPredictor[x, y] > 1 && _result.TrackNodes.Contains(new(x, y)))
                    {
                        Vector3 pos = new(x, 0, y);
                        Handles.Label(pos, $"Junction: ${_result.JunctionPredictor[x, y]}");
                    }
                    if (_result.JunctionPredictor[x, y] != 0)
                    {
                        Vector3 pos = new(x, 0, y);
                        Gizmos.DrawWireSphere(pos, 0.3f);
                    }
                }
            }

            Gizmos.color = Color.darkKhaki;
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if (_result.OccupiedMap.IsOccupied(new(x, y)))
                    {
                        if (Random.value < 0.05f)
                        {
                            Gizmos.DrawWireSphere(new(x, 0, y), 0.1f);
                        }
                    }
                }
            }
        }

        public void Initialize(IServiceRegistry serviceRegistry)
        {
            _worldGen = serviceRegistry.GetService<WorldGeneration2>();
        }
    }
}