using System;
using System.Collections;
using DG.Tweening;
using Jackie.Soft;
using UnityEngine;

public class IGCTileDestroy : InGamePowerUpContainer<InGamePowerUpConfig>
{
    [SerializeField] private float flyTime;
    [SerializeField] private RectTransform start;

    public override IEnumerator IEExecute(GameManager gameManager, Vector2Int coordinate, bool isUpdateCount = true)
    {
        var board = gameManager.Board;
        var tile = board[coordinate];

        for (var i = 0; i < Board.Depth; i++)
        {
            var color = tile[i];
            if (color != Color.NONE)
            {
                Message.Use<Type>().Event(typeof(ColorChallenge))
                    .Execute<ColorChallenge>(execution => execution.Check(1, color));
                
            }

            tile[i] = Color.NONE;
        }

        board[coordinate] = tile;
        gameManager.Board = board;

        if (!HaveCamera()) goto update_anim_for_tile;

        var tileSize = gameManager.BoardEntity.TileSize;
        var endPoint = gameManager.BoardEntity[coordinate].transform.position + new Vector3(-tileSize.x / 3, tileSize.x / 4);

        var gameObject = PoolManager.Instance["HammerEffect"].Get<HammerEffect>();
        var sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.sortingOrder = 60;
        sprite.sortingLayerName = "Dark";
        gameObject.transform.position = endPoint;

        var deltaRotate = 35f;
        var duration = 0.2f;
        Vector3 LeftTarget = new Vector3(0, 0, deltaRotate);
        Vector3 RightTargetRotate = new Vector3(0, 0, -deltaRotate);
        AudioManager.Instance.PlaySound(EventSound.Hammer);
        Tween tween = gameObject.transform.DORotate(LeftTarget, duration).OnComplete(() => {
            gameObject.transform.DORotate(RightTargetRotate, duration).OnComplete(() =>
            {
                gameObject.Release();
            });
        });

        yield return new WaitForSeconds(duration * 2);

       
        update_anim_for_tile:
        for (var i = 0; i < Board.Depth; i++)
        {
            var entity = gameManager.BoardEntity[coordinate][i];
            if(entity is not RingEntity ringEntity) continue;
            ringEntity.ReleaseBroken();
            gameManager.BoardEntity[coordinate][i] = null;
        }
        

        yield return base.IEExecute(gameManager, coordinate, isUpdateCount);
    }
}