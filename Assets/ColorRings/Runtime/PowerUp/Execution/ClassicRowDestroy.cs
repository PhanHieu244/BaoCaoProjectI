using System;
using System.Collections;
using Jackie.Soft;
using UnityEngine;

[Serializable]
public class ClassicRowDestroy : IDestroyExecute
{
    [SerializeField] private float flyTime = 0.4f;
    [SerializeField] private float divideLightScale = 2f;
    public IEnumerator Execute(GameManager gameManager, Vector2Int coordinate, bool isUpdateCount = true)
    {
        var board = gameManager.Board;
        for (var x = 0; x < board.Width; x++)
        {
            if(!board.HasTile(x, coordinate.y)) continue;
            var tile = board[new Vector2Int(x, coordinate.y)];
            for (var i = 0; i < Board.Depth; i++)
            {
                var color = tile[i];
                if (color != Color.NONE)
                    Message.Use<Type>().Event(typeof(ColorChallenge))
                        .Execute<ColorChallenge>(execution => execution.Check(1, color));

                tile[i] = Color.NONE;
            }

            board[new Vector2Int(x, coordinate.y)] = tile;
            
        }
        gameManager.Board = board;
        if (Camera.main is null) goto update_anim_for_row;

        var startCoord = new Vector2Int(gameManager.BoardEntity.Size.x / 2, coordinate.y);
        var startPoint = gameManager.BoardEntity[startCoord].transform.position;

        var leftCoord = new Vector2Int(0, coordinate.y);
        var rightCoord = new Vector2Int(gameManager.BoardEntity.Size.x - 1, coordinate.y);
        var endPointRight = gameManager.BoardEntity[leftCoord].transform.position + new Vector3(4f, 0);
        var endPointLeft = gameManager.BoardEntity[rightCoord].transform.position + new Vector3(-4f, 0);

        var gameObjectFlyToLeft = PoolManager.Instance["LeftHorizontalEffect"].Get<VerticalEffect>();
        gameObjectFlyToLeft.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        var leftSprite = gameObjectFlyToLeft.GetComponent<SpriteRenderer>();
        leftSprite.sortingOrder = 60;
        leftSprite.sortingLayerName = "Dark";

        var gameObjectFlyToRight = PoolManager.Instance["LeftHorizontalEffect"].Get<VerticalEffect>();
        gameObjectFlyToRight.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        var rightSprite = gameObjectFlyToRight.GetComponent<SpriteRenderer>();
        rightSprite.sortingOrder = 60;
        rightSprite.sortingLayerName = "Dark";

        var effect = PoolManager.Instance["horizontalLight"].Get<horizontalLight>();
        effect.transform.position = startPoint;

        var time = 0f;
        AudioManager.Instance.PlaySound(EventSound.Rocket);
        while (time < flyTime)
        {
            gameObjectFlyToLeft.transform.position = Vector3.Lerp(startPoint, endPointLeft, time / flyTime);
            gameObjectFlyToRight.transform.position = Vector3.Lerp(startPoint, endPointRight, time / flyTime);
            var distance = Vector2.Distance(gameObjectFlyToLeft.transform.position, gameObjectFlyToRight.transform.position);
            effect.gameObject.transform.localScale = new Vector2(distance / divideLightScale, effect.transform.localScale.y);
            time += Time.deltaTime;
            yield return null;
        }

        gameObjectFlyToRight.transform.position = endPointRight;
        gameObjectFlyToLeft.transform.position = endPointLeft;
        gameObjectFlyToLeft.Release();
        gameObjectFlyToRight.Release();
        effect.Release();

        // Update To Board
        update_anim_for_row:
        for (var x = 0; x < board.Width; x++)
        {
            for (var i = 0; i < Board.Depth; i++)
            {
                if (gameManager.BoardEntity[new Vector2Int(x, coordinate.y)][i] != null)
                {
                    gameManager.BoardEntity[new Vector2Int(x, coordinate.y)][i].Release();
                    gameManager.BoardEntity[new Vector2Int(x, coordinate.y)][i] = null;
                }
            }
        }
    }
}