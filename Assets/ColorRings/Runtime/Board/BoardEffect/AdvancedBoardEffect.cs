using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class AdvancedBoardEffect : BoardEffect
{
    public override void FlashEffect(IMatch matchStrategy, Color color, Direction direction)
    {
        var coordinate = matchStrategy.CoordinateCheck;
        foreach (var (position, rotationZ) in IECalculate(matchStrategy, direction))
        {
            var visualEffect = PlayEffect(coordinate, "flash", color);
            visualEffect.transform.position = position;
            var visualEffectMain = visualEffect.main;
            var radZ = rotationZ / 180 * Mathf.PI;
            visualEffectMain.startRotation = (radZ);
            DOVirtual.DelayedCall(1f, () => PoolManager.Instance["flash"].Release(visualEffect));
        }
    }

    private IEnumerable<(Vector3 position, float rotationZ)> IECalculate(IMatch matchStrategy, Direction direction)
    {
        Vector2Int start, end;
        Vector3 pos, startPos, endPos;
        if ((direction & Direction.VERTICAL) != 0)
        {
            yield return IEDirect(Direction.VERTICAL);
        }

        if ((direction & Direction.HORIZONTAL) != 0)
        {
            yield return IEDirect(Direction.HORIZONTAL);
        }

        if ((direction & Direction.DIRECT_DIAGONAL) != 0)
        {
            yield return IEDirect(Direction.DIRECT_DIAGONAL);
        }

        if ((direction & Direction.INVERSE_DIAGONAL) != 0)
        {
            yield return IEDirect(Direction.INVERSE_DIAGONAL);
        }

        (Vector3 position, float rotationZ) IEDirect(Direction direct)
        {
            start = matchStrategy.GetMatchCoordinate(direct, true);
            end = matchStrategy.GetMatchCoordinate(direct, false);
            startPos = _boardBaseEntity[start].transform.position;
            endPos = _boardBaseEntity[end].transform.position;
            pos = (startPos + endPos) / 2f;
            var sign = endPos.x - startPos.x >= 0 ? 1 : -1;
            var angle = Vector3.Angle(Vector3.up, sign * (endPos - startPos));
            return (pos, angle);
        }
    }
    
    
}
