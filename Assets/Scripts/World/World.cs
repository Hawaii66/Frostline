using System.Collections.Generic;
using UnityEngine;

class World
{
    public TileGrid tiles;
    public List<Structure> structures;

    public World(int sizeX, int sizeY)
    {
        tiles = new (sizeX, sizeY);
        structures = new();
    }

    public bool TryAddStructure(Structure structure)
    {
        if (tiles.CanPlaceStructure(structure))
        {
            tiles.PlaceStructure(structure);
            structures.Add(structure);
            return true;
        }
        else
        {
            return false;
        }
    }
}

class StructureBlueprint
{
    public Vector2Int position;
    public Vector2Int[] occupiedOffsets;

    public StructureBlueprint(Vector2Int pos, Vector2Int[] offsets)
    {
        position = pos;
        occupiedOffsets = offsets;
    }

    public Tile[] ToTiles(Vector2Int worldPosition)
    {
        Tile[] tiles = new Tile[occupiedOffsets.Length];
        for(int i = 0; i < occupiedOffsets.Length; i++)
        {
            tiles[i] = new Tile(occupiedOffsets[i] + worldPosition);
        }

        return tiles;
    }
}

class Structure
{
    public StructureBlueprint blueprint;
    public Tile[] tiles;
    public Vector2Int position;

    public Structure(StructureBlueprint blueprint, Vector2Int worldPosition)
    {
        this.blueprint = blueprint;
        position = worldPosition;
        tiles = blueprint.ToTiles(worldPosition);
    }
}

class Tile
{
    public Vector2Int position;
    public Structure structure;

    public Tile(Vector2Int pos) 
    {
        position = pos;
    }
    
    public bool IsOccupied => structure != null;
}

class TileGrid : Grid<Tile>
{
    public TileGrid(int sizeX, int sizeY) : base(sizeX, sizeY, pos => new Tile(pos)) { }
    public bool IsOccupied(int x, int y)
    {
        return cells[x, y].IsOccupied;
    }
    public bool IsOccupied(Vector2Int position)
    {
        return IsOccupied(position.x, position.y);
    }
    public bool CanPlaceStructure(Structure structure)
    {
        for(int i = 0; i < structure.tiles.Length; i++)
        {
            if (IsOccupied(structure.tiles[i].position))
            {
                return false;
            }
        }

        return true;
    }
    public void PlaceStructure(Structure structure)
    {
        for(int i = 0; i < structure.tiles.Length; i++)
        {
            Tile tile = new (structure.tiles[i].position);
            tile.structure = structure;
            SetCell(structure.tiles[i].position, tile);
        }
    }
}

delegate T CellFactory<T>(Vector2Int position);
class Grid<T>
{
    public int SizeX;
    public int SizeY;
    public int Size => SizeX * SizeY;

    public T[,] cells;

    public Grid(int sizeX, int sizeY, CellFactory<T> createCell)
    {
        SizeX = sizeX;
        SizeY = sizeY;

        cells = new T[SizeX, SizeY];

        for(int x = 0; x < SizeX; x++)
        {
            for(int y = 0; y < SizeY; y++)
            {
                cells[x,y] = createCell(new Vector2Int(x, y));
            }
        }
    }

    public T GetCell(int x, int y)
    {
        return cells[x, y];
    }
    public T GetCell(Vector2Int position)
    {
        return GetCell(position.x, position.y);
    }
    public void SetCell(int x, int y, T cell)
    {
        cells[x, y] = cell;
    }
    public void SetCell(Vector2Int position, T cell)
    {
        SetCell(position.x, position.y, cell);
    }
}