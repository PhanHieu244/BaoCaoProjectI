using System;
using UnityEngine;

[Serializable]
public class CustomRowDestroyExecute : CustomBoardDestroyExecute
{
    protected override Vector3 TargetDeltaPos => new (deltaTargetPos, 0);
    protected override Vector2Int IncreaseCoord => new (1, 0);
    protected override Vector2Int IncreaseRotateCoord => new(1, -1);
    protected override float ZEulerEffect => 180f;
    protected override Vector2Int StartCoord(Vector2Int coordinate) => new (0, coordinate.y);

    protected override Vector2Int StartRotateCoord(Vector2Int coordinate)
    {
        var boardHeight = gameManager.Board.Height - 1;
        var x = Mathf.Max(coordinate.x - (boardHeight - coordinate.y), 0);
        var y = Mathf.Min(coordinate.y + coordinate.x - x, boardHeight);
        return new Vector2Int(x, y);
    }
}