using System;
using System.Collections.Generic;
using UnityEngine;

public interface IMatch
{
    /// <summary>
    /// Get first or last coordinate of match by direction
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="isFirst">get first or last</param>
    Vector2Int GetMatchCoordinate(Direction direction, bool isFirst);
    Vector2Int CoordinateCheck { get; }
    void Initialize(IBoard board, int minRingsToMatch = 3);
    void Apply(Match match);
    List<Match> Matches(Vector2Int coordinate, Color color, out Direction direction);
}

[Serializable]
public abstract class MatchBase : IMatch
{
    protected IBoard board;
    protected int width;
    protected int height;
    protected int depth;
    protected Vector2Int coordinateCheck;
    protected Color colorCheck;
    protected int combo;
    protected List<Match> listMatch;

    public abstract Vector2Int GetMatchCoordinate(Direction direction, bool isFirst);

    public Vector2Int CoordinateCheck => coordinateCheck;

    public virtual void Initialize(IBoard board, int minRingsToMatch)
    {
        this.board = board;
        width = board.Width;
        height = board.Height;
        depth = board.DEPTH;
        coordinateCheck = default;
        colorCheck = Color.NONE;
        combo = 0;
        listMatch = null;
    }

    public void Apply(Match match)
    {
        var tile = board[match.Coordinate];
        for (var i = 0; i < match.Size.Length; i++)
        {
            tile[match.Size[i]] = Color.NONE;
        }
    }

    public abstract List<Match> Matches(Vector2Int coordinate, Color color, out Direction direction);
    protected virtual void InitCheckData(Vector2Int coordinate, Color color)
    {
        listMatch = new List<Match>();
        coordinateCheck = coordinate;
        colorCheck = color;
        combo = 0;
    }
    
    protected virtual bool CheckInside(int[] size)
    {
        if (size.Length != depth) return false;
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                if(!board.HasTile(x, y)) continue;
                var checkSize = board[x, y].Contain(colorCheck);
                if (checkSize.Length <= 0) continue;
                listMatch.Add(new Match
                {
                    Coordinate = new Vector2Int(x, y),
                    Size = checkSize,
                });
            }
        }
        return true;
    }

    protected bool IsValid(int x, int y) => board.HasTile(x, y);
    protected bool IsValid(Vector2Int coordinate) => IsValid(coordinate.x, coordinate.y);
}

[Serializable]
public class NormalMatch : MatchBase
{
    protected bool CheckOther() => (coordinateCheck.y * width + coordinateCheck.x) % 2 != 0;

    public override Vector2Int GetMatchCoordinate(Direction direction, bool isFirst)
    {
        return new Vector2Int(-1, -1);
    }

    public override List<Match> Matches(Vector2Int coordinate, Color color, out Direction direction)
    {
        InitCheckData(coordinate, color);
        direction = Direction.THIS;
        // check myself
        var size = board[coordinateCheck].Contain(colorCheck);
        if (CheckInside(size)) return listMatch;
        // check horizontal
        direction = CheckHorizontal(direction);
        // check vertical
        direction = CheckVertical(direction);
        // check others
        if(CheckOther())
        {
            CheckCombo(size);
            return listMatch;
        }
        // check x, y direct
        direction = CheckDirect(direction);
        // check x, y inverse
        direction = CheckInverse(direction);
        // check combo
        CheckCombo(size);
        return listMatch;
    }
    
    #region DirectionCheck
    protected Direction CheckHorizontal(Direction direction)
    {
        
        var xSize = new int[3][];
        for (var x = 0; x < width; x++)
        {
            if (!board.HasTile(x, coordinateCheck.y)) return direction;
            xSize[x] = board[x, coordinateCheck.y].Contain(colorCheck);
            if (xSize[x].Length == 0) return direction;
        }

        for (var x = 0; x < width; x++)
        {
            if (x == coordinateCheck.x) continue;
            listMatch.Add(new Match
            {
                Coordinate = new Vector2Int(x, coordinateCheck.y),
                Size = xSize[x],
            });
        }

        direction |= Direction.HORIZONTAL;
        combo++;
        return direction;
    }

    protected Direction CheckVertical(Direction direction)
    {
        var ySize = new int[3][];
        for (var y = 0; y < height; y++)
        {
            if (!board.HasTile(coordinateCheck.x, y)) return direction;
            ySize[y] = board[coordinateCheck.x, y].Contain(colorCheck);
            if (ySize[y].Length == 0) return direction;
        }

        for (var y = 0; y < height; y++)
        {
            if (y == coordinateCheck.y) continue;
            listMatch.Add(new Match
            {
                Coordinate = new Vector2Int(coordinateCheck.x, y),
                Size = ySize[y],
            });
        }

        direction |= Direction.VERTICAL;
        combo++;
        return direction;
    }

    protected Direction CheckDirect(Direction direction)
    {
        // check x, y direct
        var coordinateDirect = Array.Empty<Vector2Int>();
        var sizeDirect = Array.Empty<int[]>();

        var copy = coordinateCheck;
        while (IsValid(copy.x + 1, copy.y + 1))
        {
            Array.Resize(ref coordinateDirect, coordinateDirect.Length + 1);
            coordinateDirect[^1] = new Vector2Int(copy.x + 1, copy.y + 1);

            Array.Resize(ref sizeDirect, sizeDirect.Length + 1);
            sizeDirect[^1] = board[coordinateDirect[^1]].Contain(colorCheck);

            copy.x++;
            copy.y++;
            if (sizeDirect[^1].Length == 0) return direction;
        }

        copy = coordinateCheck;
        while (IsValid(copy.x - 1, copy.y - 1))
        {
            Array.Resize(ref coordinateDirect, coordinateDirect.Length + 1);
            coordinateDirect[^1] = new Vector2Int(copy.x - 1, copy.y - 1);

            Array.Resize(ref sizeDirect, sizeDirect.Length + 1);
            sizeDirect[^1] = board[coordinateDirect[^1]].Contain(colorCheck);

            copy.x--;
            copy.y--;
            if (sizeDirect[^1].Length == 0) return direction;
        }

        if (coordinateDirect.Length < 2) return direction;

        for (var i = 0; i < 2; i++)
        {
            listMatch.Add(new Match
            {
                Coordinate = coordinateDirect[i],
                Size = sizeDirect[i],
            });
        }

        direction |= Direction.DIRECT_DIAGONAL;
        combo++;
        return direction;
    }

    protected Direction CheckInverse(Direction direction)
    {
        // check x, y inverse
        var coordinateInverse = Array.Empty<Vector2Int>();
        var sizeInverse = Array.Empty<int[]>();
        var copy = coordinateCheck;
        while (IsValid(copy.x - 1, copy.y + 1))
        {
            Array.Resize(ref coordinateInverse, coordinateInverse.Length + 1);
            coordinateInverse[^1] = new Vector2Int(copy.x - 1, copy.y + 1);

            Array.Resize(ref sizeInverse, sizeInverse.Length + 1);
            sizeInverse[^1] = board[coordinateInverse[^1]].Contain(colorCheck);

            copy.x--;
            copy.y++;
            if (sizeInverse[^1].Length == 0) return direction;
        }

        copy = coordinateCheck;
        while (IsValid(copy.x + 1, copy.y - 1))
        {
            Array.Resize(ref coordinateInverse, coordinateInverse.Length + 1);
            coordinateInverse[^1] = new Vector2Int(copy.x + 1, copy.y - 1);

            Array.Resize(ref sizeInverse, sizeInverse.Length + 1);
            sizeInverse[^1] = board[coordinateInverse[^1]].Contain(colorCheck);

            copy.x++;
            copy.y--;
            if (sizeInverse[^1].Length == 0) return direction;
        }

        if (coordinateInverse.Length < 2) return direction;

        for (var i = 0; i < 2; i++)
        {
            listMatch.Add(new Match
            {
                Coordinate = coordinateInverse[i],
                Size = sizeInverse[i],
            });
        }

        direction |= Direction.INVERSE_DIAGONAL;
        combo++;
        return direction;
    }

    protected void CheckCombo(int[] size)
    {
        if (combo > 0)
        {
            listMatch.Add(new Match
            {
                Coordinate = coordinateCheck,
                Size = size,
            });
        }
    }
    
    #endregion
    
}