using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AdvancedMatch : MatchBase
{
    private int _minRingsToMatch;
    private Match _mainMatch;

    private static readonly Dictionary<Direction, Vector2Int> StepDictionary = new()
    {
        {Direction.HORIZONTAL, new Vector2Int(1, 0)},
        {Direction.VERTICAL, new Vector2Int(0, 1)},
        {Direction.DIRECT_DIAGONAL, new Vector2Int(1, 1)},
        {Direction.INVERSE_DIAGONAL, new Vector2Int(1, -1)},
    };

    private Dictionary<Direction, Vector2Int> _matchesDictionary;


    public override Vector2Int GetMatchCoordinate(Direction direction, bool isFirst)
    {
        if (!_matchesDictionary.ContainsKey(direction))
        {
            return new Vector2Int(-1, -1);
        }

        var index = isFirst ? _matchesDictionary[direction].x : _matchesDictionary[direction].y;
        if (index >= listMatch.Count)
        {
            var a = listMatch[_matchesDictionary[direction].x * 0];
            Debug.Log(isFirst);
        }
        return listMatch[index].Coordinate;
    }

    public override void Initialize(IBoard board, int minRingsToMatch)
    {
        base.Initialize(board, minRingsToMatch);
        _minRingsToMatch = minRingsToMatch;
    }

    protected override void InitCheckData(Vector2Int coordinate, Color color)
    {
        base.InitCheckData(coordinate, color);
        _matchesDictionary = new Dictionary<Direction, Vector2Int>();
    }

    public override List<Match> Matches(Vector2Int coordinate, Color color, out Direction direction)
    {
        InitCheckData(coordinate, color);
        direction = Direction.THIS;
        // check myself
        var size = board[coordinateCheck].Contain(colorCheck);
        if (CheckInside(size)) return listMatch;
        
        _mainMatch = new Match
        {
            Coordinate = coordinate,
            Size = size
        };
        listMatch.Add(_mainMatch);
        
        direction |= CheckDirection(Direction.VERTICAL);
        direction |= CheckDirection(Direction.HORIZONTAL);
        direction |= CheckDirection(Direction.INVERSE_DIAGONAL);
        direction |= CheckDirection(Direction.DIRECT_DIAGONAL);

        return combo <= 0 ? new List<Match>() : listMatch;
    }

    protected Direction CheckDirection(Direction matchDirection)
    {
        var matchesLinkedList = new LinkedList<Match>();
        var count = 0;
        var firstIndex = 0;
        var lastIndex = 0;
        var xStep = StepDictionary[matchDirection].x;
        var yStep = StepDictionary[matchDirection].y;
        
        var x = coordinateCheck.x - xStep;
        var y = coordinateCheck.y - yStep;
        while (board.HasTile(x, y))
        {
            var size = board[x, y].Contain(colorCheck);
            if (size.Length == 0) break;
            matchesLinkedList.AddFirst(new Match
            {
                Coordinate = new Vector2Int(x, y),
                Size = size
            });
            x -= xStep;
            y -= yStep;
            firstIndex = listMatch.Count;
            count++;
        }
        
        x = coordinateCheck.x + xStep;
        y = coordinateCheck.y + yStep;
        while (board.HasTile(x, y))
        {
            var size = board[x, y].Contain(colorCheck);
            if (size.Length == 0) break;
            matchesLinkedList.AddLast(new Match
            {
                Coordinate = new Vector2Int(x, y),
                Size = size
            });
            x += xStep;
            y += yStep;
            lastIndex = listMatch.Count + count;
            count++;
        }
        count++; //increase by main match
        
        //check can matches
        if (count < _minRingsToMatch) return Direction.NONE;

        _matchesDictionary[matchDirection] = new Vector2Int(firstIndex, lastIndex);
        foreach (var match in matchesLinkedList)
        {
            listMatch.Add(match);
        }
        combo++;
        return matchDirection;
    }

}
