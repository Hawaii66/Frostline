using Frostline.World;
using Frostline.World.Structures;
using Frostline.World.Tracks;
using System.Collections.Generic;
using UnityEngine;
using static Frostline.Core.World.WorldGenerationContext;

namespace Frostline.Core.World
{
    public class JunctionCell
    {
        public enum JunctionCellType
        {
            Empty,
            Junction,
            Track,
            Endpoint
        }

        public Vector2Int Position;
        public JunctionCellType Type;
        public JunctionCell[] Neighbours;

        public JunctionCell(Vector2Int position, JunctionCellType type)
        {
            Position = position;
            Type = type;
        }

        public JunctionCell Neighbour(int cardinalOffsetIndex)
        {
            return Neighbours[cardinalOffsetIndex];
        }

        public void Make(JunctionCellType type)
        {
            if (type == JunctionCellType.Track && Type == JunctionCellType.Track)
            {
                Type = JunctionCellType.Junction;
            }
            else
            {
                Type = type;
            }
        }

        public bool Is(JunctionCellType type)
        {
            return Type == type;
        }
    }

    public class DebugText
    {
        public string Text;
        public Vector2Int Position;
    }


    public class WorldGeneration3 : IRequireServices
    {
        private TrackSegmentManager _trackSegmentManager;
        private StructureBlueprintManager _blueprintManager;
        private WorldSettings _worldSettings;

        private List<IWorldGenerationStep> _steps;

        public WorldGenerationContext Generate()
        {
            WorldGenerationContext context = new(_trackSegmentManager, _blueprintManager, _worldSettings);

            SetupGeneration();
            for (int i = 0; i < _steps.Count; i++)
            {
                _steps[i].Execute(context);
            }

            return context;
        }
        public void Initialize(IServiceRegistry serviceRegistry)
        {
            _trackSegmentManager = serviceRegistry.GetService<TrackSegmentManager>();
            _blueprintManager = serviceRegistry.GetService<StructureBlueprintManager>();
            _worldSettings = serviceRegistry.GetService<WorldSettings>();
        }

        private void SetupGeneration()
        {
            _steps = new()
            {
                 new WorldGenerationStepJunctionCellSetup(),
                 new WorldGenerationStepAddCenter(),
                 new WorldGenerationStepAddStructures(),
                 new WorldGenerationStepRemoveOccupiedMapFiller(),
                 new WorldGenerationStepDistanceField(),
                 new WorldGenerationStepAddExtraJunctionCells()
            };
        }
    }

    public class WorldGenerationStepAddExtraJunctionCells : IWorldGenerationStep
    {
        public void Execute(WorldGenerationContext context)
        {
            context.ExtraJunctions = new();

            HashSet<Vector2Int> visitedJunctionCells = new();
            while (context.DistanceFieldCells.Count > 0)
            {
                DistanceFieldCell cell = context.DistanceFieldCells[^1];
                context.DistanceFieldCells.RemoveAt(context.DistanceFieldCells.Count - 1);

                if (visitedJunctionCells.Contains(cell.Position))
                {
                    continue;
                }

                HashSet<Vector2Int> trackPositions = TrackPositions(cell.Position, context);
                visitedJunctionCells.UnionWith(trackPositions);

                TryAddJunctionCells(cell.Position, context);
            }
        }

        private bool TryAddJunctionCells(Vector2Int start, WorldGenerationContext context)
        {
            JunctionCell cell = context.JunctionCells[start.x, start.y];
            for (int i = 0; i < Util.CardinalOffsets.Length; i++)
            {
                if (!cell.Neighbour(i).Is(JunctionCell.JunctionCellType.Empty))
                {
                    continue;
                }

                Vector2Int offset = Util.CardinalOffsets[i];
                Vector2Int position = start;

                HashSet<Vector2Int> onNewEdge = new();

                while (true)
                {
                    position += offset;
                    if (context.IsOutideWorld(position))
                    {
                        break;
                    }
                    if (context.OccupiedMap.IsOccupied(position))
                    {
                        break;
                    }

                    onNewEdge.Add(position);

                    JunctionCell edgeCell = context.JunctionCells[position.x, position.y];
                    if (edgeCell.Is(JunctionCell.JunctionCellType.Track))
                    {
                        if (context.DistanceField[position.x, position.y] < context.WorldSettings.MinimumDistanceBetweenJunctions)
                        {
                            break;
                        }
                        if (Random.value > context.WorldSettings.ExtraJunctionSurvivalRate)
                        {
                            break;
                        }

                        foreach (Vector2Int onEdgePosition in onNewEdge)
                        {
                            context.JunctionCells[onEdgePosition.x, onEdgePosition.y].Make(JunctionCell.JunctionCellType.Track);
                        }
                        context.JunctionCells[start.x, start.y].Make(JunctionCell.JunctionCellType.Junction);
                        context.ExtraJunctions.Add(new Edge(start, position));
                        return true;
                    }
                }
            }

            return false;
        }

        private HashSet<Vector2Int> TrackPositions(Vector2Int start, WorldGenerationContext context)
        {
            Queue<Vector2Int> queue = new();
            queue.Enqueue(start);

            HashSet<Vector2Int> visited = new();
            while (queue.Count > 0)
            {
                Vector2Int position = queue.Dequeue();
                if (visited.Contains(position))
                {
                    continue;
                }
                visited.Add(position);

                JunctionCell cell = context.JunctionCells[position.x, position.y];
                for (int i = 0; i < cell.Neighbours.Length; i++)
                {
                    JunctionCell junctionCell = cell.Neighbours[i];
                    if (junctionCell == null)
                    {
                        continue;
                    }
                    if (!junctionCell.Is(JunctionCell.JunctionCellType.Track))
                    {
                        continue;
                    }
                    queue.Enqueue(junctionCell.Position);
                }
            }

            return visited;
        }
    }

    public class WorldGenerationStepDistanceField : IWorldGenerationStep
    {
        public void Execute(WorldGenerationContext context)
        {
            int[,] distanceField = new int[context.WorldSettings.Size.x, context.WorldSettings.Size.y];
            Queue<Vector2Int> queue = new();

            for (int x = 0; x < context.WorldSettings.Size.x; x++)
            {
                for (int y = 0; y < context.WorldSettings.Size.y; y++)
                {
                    JunctionCell cell = context.JunctionCells[x, y];
                    if (cell.Is(JunctionCell.JunctionCellType.Junction))
                    {
                        queue.Enqueue(new(x, y));
                        distanceField[x, y] = 1;
                    }
                    if (cell.Is(JunctionCell.JunctionCellType.Endpoint))
                    {
                        queue.Enqueue(new(x, y));
                        distanceField[x, y] = 1;
                    }
                    if (context.IsOnEdge(new(x, y)) && !cell.Is(JunctionCell.JunctionCellType.Empty))
                    {
                        queue.Enqueue(new(x, y));
                        distanceField[x, y] = 1;
                    }
                }
            }

            HashSet<Vector2Int> visited = new();
            while (queue.Count > 0)
            {
                Vector2Int position = queue.Dequeue();
                if (visited.Contains(position))
                {
                    continue;
                }
                visited.Add(position);

                int nextDistance = distanceField[position.x, position.y] + 1;
                JunctionCell cell = context.JunctionCells[position.x, position.y];
                for (int i = 0; i < cell.Neighbours.Length; i++)
                {
                    JunctionCell junctionCell = cell.Neighbours[i];
                    if (junctionCell == null)
                    {
                        continue;
                    }
                    if (junctionCell.Is(JunctionCell.JunctionCellType.Empty))
                    {
                        continue;
                    }

                    distanceField[junctionCell.Position.x, junctionCell.Position.y] = nextDistance;
                    queue.Enqueue(junctionCell.Position);
                }
            }

            context.DistanceField = distanceField;

            List<DistanceFieldCell> distanceFieldCells = new();
            for (int x = 0; x < context.WorldSettings.Size.x; x++)
            {
                for (int y = 0; y < context.WorldSettings.Size.y; y++)
                {
                    int distance = distanceField[x, y];
                    if (distance < context.WorldSettings.MinimumDistanceBetweenJunctions)
                    {
                        continue;
                    }

                    distanceFieldCells.Add(new DistanceFieldCell(new(x, y), distance));
                }
            }

            distanceFieldCells.Sort((a, b) => a.Distance.CompareTo(b.Distance));
            context.DistanceFieldCells = distanceFieldCells;
        }
    }

    public class Edge
    {
        public Vector2Int A;
        public Vector2Int B;
        public Edge(Vector2Int a, Vector2Int b)
        {
            A = a;
            B = b;
        }
    }
    public interface IWorldGenerationStep
    {
        public void Execute(WorldGenerationContext context);
    }
    public struct DistanceFieldCell
    {
        public Vector2Int Position;
        public int Distance;

        public DistanceFieldCell(Vector2Int position, int distance)
        {
            Position = position;
            Distance = distance;
        }
    }
    public class WorldGenerationContext
    {
        public TrackSegmentManager TrackSegmentManager;
        public StructureBlueprintManager StructureBlueprintManager;
        public WorldSettings WorldSettings;

        public OccupiedMap<IPlaceable> OccupiedMap;
        public List<Structure> Structures;
        public List<Vector2Int> TrackNodes;
        public List<Edge> TrackEdges;
        public List<DebugText> DebugTexts;

        public Structure CenterStructure;
        public JunctionCell[,] JunctionCells;
        public int[,] DistanceField;
        public List<DistanceFieldCell> DistanceFieldCells;
        public List<Edge> ExtraJunctions;

        public WorldGenerationContext(TrackSegmentManager tsm, StructureBlueprintManager sbm, WorldSettings ws)
        {
            TrackSegmentManager = tsm;
            StructureBlueprintManager = sbm;
            WorldSettings = ws;

            OccupiedMap = new();
            Structures = new();
            TrackNodes = new();
            TrackEdges = new();
            DebugTexts = new();
        }

        public bool IsOutideWorld(Vector2Int position)
        {
            return position.x < 0 || position.y < 0 || position.x > WorldSettings.Size.x - 1 || position.y > WorldSettings.Size.y - 1;
        }
        public bool IsInsideWorld(Vector2Int position)
        {
            return !IsOutideWorld(position);
        }
        public bool IsOnEdge(Vector2Int position)
        {
            return position.x == 0 || position.y == 0 || position.x == WorldSettings.Size.x - 1 || position.y == WorldSettings.Size.y - 1;
        }

        public void AddInternalTrackPaths(Structure structure, StructureBlueprintTrack structureBlueprintTrack)
        {
            TrackPath[] trackPaths = structureBlueprintTrack.TrackPaths;
            for (int i = 0; i < trackPaths.Length; i++)
            {
                Vector2Int[] path = trackPaths[i].Path;
                Vector2Int centerOffset = trackPaths[i].CenterOffset;

                Vector2Int[] transformedPosition = new Vector2Int[path.Length];
                for (int j = 0; j < path.Length; j++)
                {
                    Vector2Int position = RotationManager.Rotate(path[j] + centerOffset, structure.Rotation) + structure.WorldPosition;
                    TrackNodes.Add(position);
                    transformedPosition[j] = position;
                }

                for (int j = 0; j < transformedPosition.Length - 2; j++)
                {
                    TrackEdges.Add(new Edge(transformedPosition[i], transformedPosition[i + 1]));
                }
            }
        }
        public void ExpandInternalTrackPaths(Structure structure, StructureBlueprintTrack structureBlueprintTrack)
        {
            TrackPath[] trackPaths = structureBlueprintTrack.TrackPaths;
            for (int i = 0; i < trackPaths.Length; i++)
            {
                Vector2Int[] path = trackPaths[i].Path;
                Vector2Int centerOffset = trackPaths[i].CenterOffset;

                Vector2Int lastPosition = RotationManager.Rotate(path[^1] + centerOffset, structure.Rotation) + structure.WorldPosition;
                Vector2Int seccondLastPosition = RotationManager.Rotate(path[^2] + centerOffset, structure.Rotation) + structure.WorldPosition;
                Vector2Int moveDirection = new(Util.Sign(lastPosition.x - seccondLastPosition.x), Util.Sign(lastPosition.y - seccondLastPosition.y));

                Vector2Int position = lastPosition;
                JunctionCells[position.x, position.y].Make(JunctionCell.JunctionCellType.Endpoint);

                position += moveDirection;
                while (IsInsideWorld(position))
                {
                    JunctionCell cell = JunctionCells[position.x, position.y];
                    cell.Make(JunctionCell.JunctionCellType.Track);

                    if (cell.Is(JunctionCell.JunctionCellType.Junction))
                    {
                        break;
                    }
                    position += moveDirection;
                }
            }
        }

        public void ExpandCardinalStructureBounds(Vector2Int[] bounds)
        {
            HashSet<Vector2Int> visited = new();
            for (int i = 0; i < bounds.Length; i++)
            {
                for (int j = 0; j < Util.CardinalOffsets.Length; j++)
                {
                    Vector2Int offset = Util.CardinalOffsets[j];

                    Vector2Int position = bounds[i];
                    while (true)
                    {
                        position += offset;
                        if (IsOutideWorld(position)) break;
                        if (visited.Contains(position)) break;
                        visited.Add(position);

                        if (OccupiedMap.IsOccupied(position)) continue;
                        OccupiedMapFiller filler = new(position);
                        OccupiedMap.Add(filler);
                    }
                }
            }
        }



        public class WorldGenerationStepAddCenter : IWorldGenerationStep
        {
            private static readonly string CenterStructureName = "Center";
            public void Execute(WorldGenerationContext context)
            {
                if (context.StructureBlueprintManager.TryGetStructureBlueprint(CenterStructureName, out StructureBlueprint centerBlueprint))
                {
                    if (context.TrackSegmentManager.TrackSegmentDict.TryGetValue(CenterStructureName, out StructureBlueprintTrack centerBlueprintTrack))
                    {
                        Vector2Int centerPosition = context.WorldSettings.Size / 2;
                        Structure centerStructure = new(centerBlueprint, centerPosition);
                        if (context.OccupiedMap.CanPlace(centerStructure.Bounds()))
                        {
                            context.OccupiedMap.Add(centerStructure, centerStructure.Bounds());
                            context.Structures.Add(centerStructure);
                            context.CenterStructure = centerStructure;
                            context.ExpandCardinalStructureBounds(centerStructure.Bounds());
                            context.AddInternalTrackPaths(centerStructure, centerBlueprintTrack);
                            context.ExpandInternalTrackPaths(centerStructure, centerBlueprintTrack);
                        }
                    }
                }
            }
        }
        public class WorldGenerationStepJunctionCellSetup : IWorldGenerationStep
        {
            public void Execute(WorldGenerationContext context)
            {
                JunctionCell[,] junctionCells = new JunctionCell[context.WorldSettings.Size.x, context.WorldSettings.Size.y];

                for (int x = 0; x < junctionCells.GetLength(0); x++)
                {
                    for (int y = 0; y < junctionCells.GetLength(1); y++)
                    {
                        junctionCells[x, y] = new(new(x, y), JunctionCell.JunctionCellType.Empty);
                    }
                }

                for (int x = 0; x < junctionCells.GetLength(0); x++)
                {
                    for (int y = 0; y < junctionCells.GetLength(1); y++)
                    {
                        Vector2Int position = new(x, y);

                        JunctionCell[] neighbours = new JunctionCell[4];
                        for (int i = 0; i < Util.CardinalOffsets.Length; i++)
                        {
                            Vector2Int offset = Util.CardinalOffsets[i];
                            Vector2Int neighbourPosition = position + offset;
                            if (context.IsInsideWorld(neighbourPosition))
                            {
                                neighbours[i] = junctionCells[neighbourPosition.x, neighbourPosition.y];
                            }
                            else
                            {
                                neighbours[i] = null;
                            }

                        }

                        junctionCells[x, y].Neighbours = neighbours;
                    }
                }

                context.JunctionCells = junctionCells;
            }
        }
        public class WorldGenerationStepRemoveOccupiedMapFiller : IWorldGenerationStep
        {
            public void Execute(WorldGenerationContext context)
            {
                IPlaceable[] placeables = context.OccupiedMap.AsArray();
                for (int i = 0; i < placeables.Length; i++)
                {
                    if (placeables[i] is OccupiedMapFiller)
                    {
                        context.OccupiedMap.Remove(placeables[i]);
                    }
                }
            }
        }

        public class WorldGenerationStepAddStructures : IWorldGenerationStep
        {
            private readonly static string StructureName = "TestT";

            private StructureBlueprint _structureBlueprint;
            private StructureBlueprintTrack _structureBlueprintTrack;
            public void Execute(WorldGenerationContext context)
            {
                if (context.StructureBlueprintManager.TryGetStructureBlueprint(StructureName, out StructureBlueprint structureBlueprint))
                {
                    _structureBlueprint = structureBlueprint;
                }
                if (context.TrackSegmentManager.TrackSegmentDict.TryGetValue(StructureName, out StructureBlueprintTrack structureBlueprintTrack))
                {
                    _structureBlueprintTrack = structureBlueprintTrack;
                }

                for (int i = 0; i < context.WorldSettings.StructureCount; i++)
                {
                    int maxAttempts = 100;
                    for (int j = 0; j < maxAttempts; j++)
                    {
                        if (TryAddStructure(context))
                        {
                            break;
                        }
                    }
                }
            }

            private bool TryAddStructure(WorldGenerationContext context)
            {
                int structureBufferRadius = context.WorldSettings.StructureBorderBufferRadius;
                int randX = Random.Range(structureBufferRadius, context.WorldSettings.Size.x - structureBufferRadius);
                int randY = Random.Range(structureBufferRadius, context.WorldSettings.Size.y - structureBufferRadius);

                int diffX = randX - context.CenterStructure.WorldPosition.x;
                int diffY = randY - context.CenterStructure.WorldPosition.y;
                Rotation rotation = CenterOffsetToRotation(diffX, diffY);

                Vector2Int worldPosition = new(randX, randY);
                Structure structure = new(_structureBlueprint, worldPosition, rotation);
                if (!context.OccupiedMap.CanPlace(structure.Bounds()))
                {
                    return false;
                }

                context.OccupiedMap.Add(structure, structure.Bounds());
                context.Structures.Add(structure);
                context.ExpandCardinalStructureBounds(structure.Bounds());
                context.AddInternalTrackPaths(structure, _structureBlueprintTrack);
                context.ExpandInternalTrackPaths(structure, _structureBlueprintTrack);
                return true;
            }
            private Rotation CenterOffsetToRotation(int diffX,
                int diffY)
            {
                int diffXAbs = Mathf.Abs(diffX);
                int diffYAbs = Mathf.Abs(diffY);
                if (diffXAbs > diffYAbs)
                {
                    if (diffX > 0)
                    {
                        return Rotation.Right;
                    }
                    return Rotation.Left;
                }
                if (diffY > 0)
                {
                    return Rotation.Up;
                }
                return Rotation.Down;
            }
        }

        public class OccupiedMapFiller : IPlaceable
        {
            private readonly Vector2Int[] _pos;
            public OccupiedMapFiller(Vector2Int pos)
            {
                _pos = new Vector2Int[1] { pos };
            }
            public Vector2Int[] GetOccupiedPositions()
            {
                return _pos;
            }
        }
    }
}