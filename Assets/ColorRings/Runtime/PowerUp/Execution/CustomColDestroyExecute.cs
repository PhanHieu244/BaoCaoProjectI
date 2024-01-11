using System;
using UnityEngine;

[Serializable]
public class CustomColDestroyExecute : CustomBoardDestroyExecute
{
    protected override Vector3 TargetDeltaPos => new (0 , deltaTargetPos);
    protected override Vector2Int IncreaseCoord => new (0, 1);
    protected override Vector2Int IncreaseRotateCoord => new(1, 1);
    protected override float ZEulerEffect => -90f;
    protected override Vector2Int StartCoord(Vector2Int coordinate) => new (coordinate.x, 0);

    protected override Vector2Int StartRotateCoord(Vector2Int coordinate)
    {
        var x = Mathf.Max(coordinate.x - coordinate.y, 0);
        var y = Mathf.Max(coordinate.y - coordinate.x, 0);
        return new Vector2Int(x, y);
    }
}