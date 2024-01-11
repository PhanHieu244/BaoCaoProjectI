using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class ClassicBoardEffect : BoardEffect
{

    public void FlashEffect(Vector2Int coordinate, Color color, Direction direction)
    {
        foreach (var (position, rotation) in IECalculate(coordinate, direction))
        {
            var visualEffect = PlayEffect(coordinate, "flash", color);
            visualEffect.transform.localPosition = position;
            var visualEffectMain = visualEffect.main;
            var rotationZ = (rotation.z) / 180 * Mathf.PI;
            visualEffectMain.startRotation = (rotationZ);
            DOVirtual.DelayedCall(1f, () => PoolManager.Instance["flash"].Release(visualEffect));
        }
    }

    public override void FlashEffect(IMatch matchStrategy, Color color, Direction direction)
    {
        FlashEffect(matchStrategy.CoordinateCheck, color, direction);
    }

    private IEnumerable<(Vector3 position, Vector3 rotation)> IECalculate(Vector2Int coordinate, Direction direction)
    {
        if ((direction & Direction.VERTICAL) != 0)
        {
            yield return (_boardBaseEntity[coordinate.x, 1].transform.localPosition, Vector3.zero);
        }

        if ((direction & Direction.HORIZONTAL) != 0)
        {
            yield return (_boardBaseEntity[1, coordinate.y].transform.localPosition, Vector3.forward * 90);
        }

        if ((direction & Direction.DIRECT_DIAGONAL) != 0)
        {
            yield return (_boardBaseEntity[1, 1].transform.localPosition, Vector3.forward * 45);
        }

        if ((direction & Direction.INVERSE_DIAGONAL) != 0)
        {
            yield return (_boardBaseEntity[1, 1].transform.localPosition, Vector3.forward * -45);
        }
    }
}