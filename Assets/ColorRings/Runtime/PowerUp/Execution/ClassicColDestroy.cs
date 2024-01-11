using System;
using System.Collections;
using Jackie.Soft;
using UnityEngine;

[Serializable]
public class ClassicColDestroy : IDestroyExecute
{
    [SerializeField] private float flyTime = 0.4f;
    [SerializeField] private float maxDistance = 2f;
    public IEnumerator Execute(GameManager gameManager, Vector2Int coordinate, bool isUpdateCount = true)
    {
        var board = gameManager.Board;
        for (var y = 0; y < board.Height; y++)
        {
            if(!board.HasTile(coordinate.x, y)) continue;
            var tile = board[new Vector2Int(coordinate.x, y)];
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

            board[new Vector2Int(coordinate.x, y)] = tile;
        }

        gameManager.Board = board;
        
        if (Camera.main is null) goto update_anim_for_col;

        var startCoord = new Vector2Int(coordinate.x, gameManager.BoardEntity.Size.y / 2);
        var startPoint = gameManager.BoardEntity[startCoord].transform.position;
        var tileSize = gameManager.BoardEntity.TileSize;

        var leftCoord = new Vector2Int(coordinate.x, 0);
        var rightCoord = new Vector2Int(coordinate.x, gameManager.BoardEntity.Size.y - 1);
        var endPointRight = gameManager.BoardEntity[leftCoord].transform.position + new Vector3(0, 5f);
        var endPointLeft = gameManager.BoardEntity[rightCoord].transform.position + new Vector3(0, -5f);

        var time = 0f;
        var gameObjectFlyDown = PoolManager.Instance["LeftHorizontalEffect"].Get<VerticalEffect>();
        gameObjectFlyDown.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        var sprite = gameObjectFlyDown.GetComponent<SpriteRenderer>();
        sprite.sortingOrder = 60;
        sprite.sortingLayerName = "Dark";

        var gameObjectFlyUp = PoolManager.Instance["LeftHorizontalEffect"].Get<VerticalEffect>();
        gameObjectFlyUp.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
        var sprite2 = gameObjectFlyUp.GetComponent<SpriteRenderer>();
        sprite2.sortingOrder = 60;
        sprite2.sortingLayerName = "Dark";

        var effect = PoolManager.Instance["horizontalLight"].Get<horizontalLight>();
        effect.transform.position = startPoint;
        effect.transform.rotation = Quaternion.Euler(0, 0, -90f);

        AudioManager.Instance.PlaySound(EventSound.Rocket);
        while (time < flyTime)
        {
            gameObjectFlyDown.transform.position = Vector3.Lerp(startPoint, endPointLeft, time / flyTime);
            gameObjectFlyUp.transform.position = Vector3.Lerp(startPoint, endPointRight, time / flyTime);
            var distance = Vector2.Distance(gameObjectFlyUp.transform.position, gameObjectFlyDown.transform.position);
            effect.gameObject.transform.localScale = new Vector2(distance / maxDistance, effect.transform.localScale.y);
            time += Time.deltaTime;
            yield return null;
        }

        gameObjectFlyUp.transform.position = endPointRight;
        gameObjectFlyDown.transform.position = endPointLeft;
        
        gameObjectFlyDown.Release();
        gameObjectFlyUp.Release();
        effect.Release();

        // Update Anim to Board
        update_anim_for_col:
        for (var y = 0; y < board.Height; y++)
        {
            for (var i = 0; i < Board.Depth; i++)
            {
                if (gameManager.BoardEntity[new Vector2Int(coordinate.x, y)][i] is not null)
                {
                    gameManager.BoardEntity[new Vector2Int(coordinate.x, y)][i].Release();
                    gameManager.BoardEntity[new Vector2Int(coordinate.x, y)][i] = null;
                }
            }
        }
    }
}