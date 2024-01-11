using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public struct Board : IBoard
{
    public static readonly int Depth = Enum.GetValues(typeof(RingSize)).Length;
    private Tile[,] _tiles;

    public Board(Vector2Int size)
    {
        _tiles = new Tile[size.x, size.y];
        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                _tiles[x, y] = new Tile(Depth);
            }
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

    public void Apply(Match match)
    {
        var tile = this[match.Coordinate];
        for (var i = 0; i < match.Size.Length; i++)
        {
            tile[match.Size[i]] = Color.NONE;
        }
    }

    public bool HasTile(int x, int y) => x < Width && x >= 0 && y < Height && y >= 0;
    public bool HasTile(int x, int y, out bool isValidRange)
    {
        isValidRange = x < Width && x >= 0 && y < Height && y >= 0;
        return isValidRange;
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
        
    }

    public void UnlockLockTile(Vector2Int coord)
    {
        
    }

    public bool IsLock(Vector2Int coord) => false;  

    public int GetRingsAmount()
    {
        var count = 0;
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                for (int j = 0; j < Depth; j++)
                {
                    if (this[x, y][j] != Color.NONE) count++;
                }
            }
        }

        return count;
    }

    public SizePattern GetAvailablePattern(List<SizePattern> sizePatterns, Color[] patternInHolder = null)
    {
        int sizeBoard = Height * Width;
        var countSize = new int[Depth];
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                for (int j = 0; j < Depth; j++)
                {
                    if (this[x, y][j] != Color.NONE) countSize[j]++;
                }
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
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var isValid = true;
                    foreach (var size in pattern.sizes)
                    {
                        if (this[x, y][size] == Color.NONE) continue;
                        isValid = false;
                        break;
                    }
                    
                    if (!isValid) continue;
                    containASizeValid = true;
                    availPatterns.Add(pattern);
                    goto next_loop;
                }
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
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var isValid = true;
                    for (var si = 0; si < pattern.sizes.Length; si++)
                    {
                        if (this[x, y][pattern.sizes[si]] == Color.NONE) continue;
                        isValid = false;
                        break;
                    }

                    if (!isValid) continue;
                    containASizeValid = true;
                    yield return pattern;
                    goto next_loop;
                }
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

[Serializable]
public struct Match
{
    public Vector2Int Coordinate;
    public int[] Size;
}

public struct Tile
{
    public Tile(int depth)
    {
        Colors = Enumerable.Repeat(Color.NONE, depth).ToArray();
    }

    private Color[] Colors { get; set; }

    public int[] Contain(Color color)
    {
        var arr = Array.Empty<int>();
        for (var i = 0; i < Colors.Length; i++)
        {
            if (Colors[i] != color) continue;
            var size = arr.Length;
            Array.Resize(ref arr, size + 1);
            arr[^1] = i;
        }
        return arr;
    }

    public Color this[RingSize ringSize]
    {
        get => Colors[(int)ringSize];
        set => Colors[(int)ringSize] = value;
    }

    public Color this[int size]
    {
        get => Colors[size];
        set => Colors[size] = value;
    }

    public override string ToString()
    {
        return $"[{Colors[0]}|{Colors[1]}|{Colors[2]}]";
    }

    public bool IsAvailable(Color[] colors)
    {

        for (var i = 0; i < Board.Depth; i++)
        {
            if (colors[i] != Color.NONE && Colors[i] != Color.NONE) return false;
        }

        return true;
    }
}

public enum Color
{
    NONE,
    RED,
    GREEN,
    ORANGE,
    PINK,
    YELLOW,
    CYAN,
    WHITE,
    BLUE,
    PURPLE,
    COUNT
}

public enum RingSize
{
    SMALL_RING = 0,
    MEDIUM_RING,
    BIG_RING,
}

[Flags]
public enum Direction
{
    NONE = 0,
    THIS = 1 << 0,
    VERTICAL = 1 << 1,
    HORIZONTAL = 1 << 2,
    DIRECT_DIAGONAL = 1 << 3,
    INVERSE_DIAGONAL = 1 << 4,
}