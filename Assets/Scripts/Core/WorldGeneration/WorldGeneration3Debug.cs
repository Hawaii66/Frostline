using Frostline.Test;
using Frostline.World.Structures;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Frostline.Core.World
{
    public class WorldGeneration3Debug : MonoBehaviour, IRequireServices
    {
        private WorldGeneration3 _worldGeneration3;
        private WorldGenerationContext _context;

        [Button]
        private void GenerateWorld()
        {
            _context = _worldGeneration3.Generate();
        }

        private void OnDrawGizmos()
        {
            if (_context == null) { return; }

            int sizeX = _context.WorldSettings.Size.x;
            int sizeY = _context.WorldSettings.Size.y;

            Vector3 center = new(sizeX / 2, 0, sizeY / 2);
            Vector3 size = new(sizeX, 1, sizeY);
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(center, size);

            List<Structure> structures = _context.Structures;
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
                Vector2Int[] bounds = structure.Bounds();
                Vector2Int boundLowerLeft = bounds[0];
                Vector2Int boundUpperRight = bounds[bounds.Length - 1];
                Vector3 boundCenter = new((boundLowerLeft.x + boundUpperRight.x) / 2, 0, (boundLowerLeft.y + boundUpperRight.y) / 2);
                Vector3 boundSize = new(boundUpperRight.x - boundLowerLeft.x, 0.2f, boundUpperRight.y - boundLowerLeft.y);
                Gizmos.DrawWireCube(boundCenter, boundSize);
            }

            Gizmos.color = Color.wheat;
            foreach (Vector2Int position in _context.TrackNodes)
            {
                Vector3 pos = new(position.x, 0, position.y);
                Gizmos.DrawWireSphere(pos, 0.2f);
            }


            Gizmos.color = Color.darkSlateBlue;
            foreach (DebugText debugText in _context.DebugTexts)
            {
                Vector3 pos = new(debugText.Position.x, 0, debugText.Position.y);
                Handles.Label(pos, debugText.Text);
            }


            Gizmos.color = Color.blanchedAlmond;
            for (int i = 0; i < _context.TrackEdges.Count; i++)
            {
                Vector2Int start = _context.TrackEdges[i].A;
                Vector2Int end = _context.TrackEdges[i].B;
                Vector3 s = new(start.x, 0, start.y);
                Vector3 e = new(end.x, 0, end.y);
                Gizmos.DrawLine(s, e);
            }

            Gizmos.color = Color.blueViolet;
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    Vector3 pos = new(x, 0.3f, y);
                    if (_context.JunctionCells[x, y].Is(JunctionCell.JunctionCellType.Junction))
                    {
                        Gizmos.color = Color.blueViolet;
                        Gizmos.DrawWireSphere(pos, 0.3f);
                        Handles.Label(pos, "Junction");
                    }
                    if (_context.JunctionCells[x, y].Is(JunctionCell.JunctionCellType.Endpoint))
                    {
                        Gizmos.color = Color.aliceBlue;
                        Gizmos.DrawWireSphere(pos, 0.3f);
                    }
                    if (_context.JunctionCells[x, y].Is(JunctionCell.JunctionCellType.Track))
                    {
                        Gizmos.color = Color.cornflowerBlue;
                        Gizmos.DrawWireSphere(pos, 0.3f);
                    }

                }
            }

            Gizmos.color = Color.darkKhaki;
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if (_context.OccupiedMap.IsOccupied(new(x, y)))
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
            _worldGeneration3 = serviceRegistry.GetService<WorldGeneration3>();
        }
    }
}