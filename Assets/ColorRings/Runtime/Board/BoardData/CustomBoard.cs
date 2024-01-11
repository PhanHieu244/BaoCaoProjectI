using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public struct CustomBoard : IBoard
{
    private static readonly int Depth = Enum.GetValues(typeof(RingSize)).Length;
    private readonly Tile[,] _tiles;
    private readonly Vector2Int[] _availableTiles;
    private readonly bool[,] _validMatrix;

    public CustomBoard(Vector2Int size, List<Vector2Int> availableTiles)
    {
        _tiles = new Tile[size.x, size.y];
        _validMatrix = new bool[size.x, size.y];
        _availableTiles = availableTiles.ToArray();
        foreach (var availableTile in _availableTiles)
        {
            _tiles[availableTile.x, availableTile.y] = new Tile(Depth);
            _validMatrix[availableTile.x, availableTile.y] = true;
        }
    }

    public Tile this[int x, int y]
    {
        get => _tiles[x, y];
        set => _tiles[x, y] = value;
    }

    public Tile this[Vector2Int coordinate]
    {
        get => _tiles[coordinate.x, coordinate.y];
        set => _tiles[coordinate.x, coordinate.y] = value;
    }

    public int Width => _tiles.GetLength(0);
    public int Height => _tiles.GetLength(1);

    
    public override string ToString()
    {
        var s = "";
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                s += _tiles[x, y].ToString();
            }

            s += "\n";
        }

        return s;
    }


    public bool HasTile(int x, int y) => x < Width && x >= 0 && y < Height && y >= 0 && _validMatrix[x, y];
    public bool HasTile(int x, int y, out bool isValidRange)
    {
        isValidRange = x < Width && x >= 0 && y < Height && y >= 0;
        return isValidRange && _validMatrix[x, y];
    }

    public bool HasRing(int x, int y)
    {
        var hasTile = HasTile(x, y);
        if (!hasTile) return false;
        var coord = new Vector2Int(x, y);
        for (int colorSizeIndex = 0; colorSizeIndex < Depth; colorSizeIndex++)
        {
            if (this[coord][colorSizeIndex] != Color.NONE) return true;
        }
        return false;
    }

    public void LockTile(Vector2Int coord)
    {
        _validMatrix[coord.x, coord.y] = false;
    }

    public void UnlockLockTile(Vector2Int coord)
    {
        _validMatrix[coord.x, coord.y] = true;
    }


    public bool IsLock(int x, int y)
    {
        return IsLock(new Vector2Int(x, y));
    }

    public bool IsLock(Vector2Int coord) => _validMatrix[coord.x, coord.y];

    public int GetRingsAmount()
    {
        var count = 0;
        foreach (var available in _availableTiles)
        {
            for (int j = 0; j < Depth; j++)
            {
                if (this[available][j] != Color.NONE) count++;
            }
        }

        return count;
    }

    public SizePattern GetAvailablePattern(List<SizePattern> sizePatterns, Color[] patternInHolder = null)
    {
        int sizeBoard = Height * Width;
        var countSize = new int[Depth];
        foreach (var available in _availableTiles)
        {
            for (int j = 0; j < Depth; j++)
            {
                if (this[available][j] != Color.NONE) countSize[j]++;
            }
        }

        if (patternInHolder is not null)
        {
            for (int j = 0; j < patternInHolder.Length; j++)
            {
                if(patternInHolder[j] == Color.NONE) continue;
                countSize[j]++;
            }
        }
        
        var availPatterns = new List<SizePattern>();
        var containASizeValid = false;
        var i = 0;
        while (i < sizePatterns.Count)
        {
            var pattern = sizePatterns[i];
            var canFit = true;
            if (pattern.sizes.Any(size => countSize[(int)size] > sizeBoard))
            {
                i++;
                canFit = false;
            }
            if(!canFit) continue;
            foreach (var availableTile in _availableTiles)
            {
                var isValid = true;
                foreach (var size in pattern.sizes)
                {
                    if (this[availableTile][size] == Color.NONE) continue;
                    isValid = false;
                    break;
                }
                    
                if (!isValid) continue;
                containASizeValid = true;
                availPatterns.Add(pattern);
                goto next_loop;
            }
            next_loop:
            i++;
        }
        
        
        return !containASizeValid ? sizePatterns[0] : availPatterns[Random.Range(0, availPatterns.Count)];
    }

    public IEnumerable<SizePattern> AvailablePattern(SizePattern[] sizePatterns)
    {
        bool containASizeValid = false;
        for (var i = 0; i < sizePatterns.Length;)
        {
            var pattern = sizePatterns[i];
            foreach (var availableTile in _availableTiles)
            {
                var isValid = true;
                for (var si = 0; si < pattern.sizes.Length; si++)
                {
                    if (this[availableTile][pattern.sizes[si]] == Color.NONE) continue;
                    isValid = false;
                    break;
                }

                if (!isValid) continue;
                containASizeValid = true;
                yield return pattern;
                goto next_loop;
            }
            next_loop:
            i++;
        }

        if (!containASizeValid)
        {
            yield return sizePatterns[0];
        }
        
    }
}