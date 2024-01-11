using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BonusBoardEffect : AdvancedBoardEffect
{
    public override void ApplyEffect(Vector2Int coordinate, Color color)
    {
        var visualEffect = PlayEffect(coordinate, "circle", color);
        DOVirtual.DelayedCall(1f, () => PoolManager.Instance["circle"].Release(visualEffect));

        var blinkEffect = PlayEffect(coordinate, "blink", color);
        DOVirtual.DelayedCall(1f, () => PoolManager.Instance["blink"].Release(blinkEffect));

        var coinEffect = PlayCoinEffect(coordinate);
        DOVirtual.DelayedCall(2.6f, () => PoolManager.Instance["DropCoin"].Release(coinEffect));
    }

    private DropCoin PlayCoinEffect(Vector2Int positionCoord)
    {
        var coinEffect = PoolManager.Instance["DropCoin"].Get<DropCoin>();
        coinEffect.DropCoins(Camera.main.WorldToScreenPoint(_boardBaseEntity[positionCoord].transform.position), new Vector2(-408, 822), 0.5f);

        return coinEffect;
    }
}
