using System;
using System.Collections;
using Jackie.Soft;
using UnityEngine;

[Serializable]
public abstract class CustomBoardDestroyExecute : IDestroyExecute
{
    [SerializeField] private float flyTime = 0.4f;
    [SerializeField] private float divideLightScale = 2f;
    [SerializeField] protected float deltaTargetPos = 5;
    
    protected abstract Vector3 TargetDeltaPos { get; } 
    protected abstract Vector2Int IncreaseCoord { get; } 
    protected abstract Vector2Int IncreaseRotateCoord { get; } 
    
    protected abstract float ZEulerEffect { get; }
    protected GameManager gameManager;
    protected Vector2Int increaseCoordByBoard;
    protected Vector2Int startCoordByBoard;

    protected abstract Vector2Int StartCoord(Vector2Int coordinate);
    protected abstract Vector2Int StartRotateCoord(Vector2Int coordinate);

    protected void SetupByBoard(Vector2Int coordinate)
    {
        if (gameManager.BoardEntity.BoardType == BoardType.Classic)
        {
            increaseCoordByBoard = IncreaseCoord;
            startCoordByBoard = StartCoord(coordinate);
            return;
        }
        increaseCoordByBoard = IncreaseRotateCoord;
        startCoordByBoard = StartRotateCoord(coordinate);
    }
    
    protected void UpdateBoard()
    {
        var board = gameManager.Board;
        var isValidRange = true;
        var coord = startCoordByBoard;
        while (isValidRange)
        {
            var hasTile = board.HasTile(coord.x, coord.y, out isValidRange);
            
            if(!hasTile)
            {
                coord += increaseCoordByBoard;
                continue;
            }
            var tile = board[coord];
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

            board[coord] = tile;
            coord += increaseCoordByBoard;
        }
        gameManager.Board = board;
    }

    protected void UpdateBoardEntity()
    {
        var isValidRange = true;
        var coord = startCoordByBoard;
        var boardEntity = gameManager.BoardEntity;
        while (isValidRange)
        {
            DevLog.Log("Coord",coord);
            var hasTile = gameManager.Board.HasTile(coord.x, coord.y, out isValidRange);

            if (!hasTile)
            {
                coord += increaseCoordByBoard;
                continue;
            }
            
            for (var i = 0; i < Board.Depth; i++)
            {
                if (boardEntity[coord][i] is null) continue;
                boardEntity[coord][i].Release();
                boardEntity[coord][i] = null;
            }

            coord += increaseCoordByBoard;
        }
    }

    public IEnumerator Execute(GameManager gameManager, Vector2Int coordinate, bool isUpdateCount = true)
    {
        DevLog.Log("In this Coord", coordinate);
        this.gameManager = gameManager;
        SetupByBoard(coordinate);
        UpdateBoard();
        
        if (Camera.main is null) goto update_anim_for_row;
        
        var startPoint = gameManager.BoardEntity[coordinate].transform.position;
        
        var endPointRight = startPoint + TargetDeltaPos;
        var endPointLeft = startPoint - TargetDeltaPos;

        var gameObjectFlyToLeft = SpawnEffect(ZEulerEffect + 180);
        var gameObjectFlyToRight = SpawnEffect(ZEulerEffect);

        var effect = PoolManager.Instance["horizontalLight"].Get<horizontalLight>();
        effect.transform.position = startPoint;
        effect.transform.rotation = Quaternion.Euler(0, 0, -ZEulerEffect);
        
        var time = 0f;
        AudioManager.Instance.PlaySound(EventSound.Rocket);
        var frontTransform = gameObjectFlyToRight.transform;
        var behindTransform = gameObjectFlyToLeft.transform;
        while (time < flyTime)
        {
            behindTransform.position = Vector3.Lerp(startPoint, endPointLeft, time / flyTime);
            frontTransform.position = Vector3.Lerp(startPoint, endPointRight, time / flyTime);
            var distance = Vector2.Distance(behindTransform.position, frontTransform.position);
            effect.gameObject.transform.localScale = new Vector2(distance / divideLightScale, effect.transform.localScale.y);
            time += Time.deltaTime;
            yield return null;
        }

        frontTransform.position = endPointRight;
        behindTransform.position = endPointLeft;
        gameObjectFlyToLeft.Release();
        gameObjectFlyToRight.Release();
        effect.Release();

        // Update To Board
        update_anim_for_row:
        UpdateBoardEntity();
    }

    protected virtual VerticalEffect SpawnEffect(float zEuler)
    {
        var verticalEffect = PoolManager.Instance["LeftHorizontalEffect"].Get<VerticalEffect>();
        verticalEffect.transform.rotation = Quaternion.Euler(0f, 0f, zEuler);
        var leftSprite = verticalEffect.GetComponent<SpriteRenderer>();
        leftSprite.sortingOrder = 60;
        leftSprite.sortingLayerName = "Dark";
        return verticalEffect;
    }
}