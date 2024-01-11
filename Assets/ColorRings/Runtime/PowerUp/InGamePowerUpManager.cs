using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class InGamePowerUpManager : MonoSingleton<InGamePowerUpManager>
{
    private IPowerUpExecution _powerUpExecutionExecution;
    [SerializeField] private GameObject powerUpInput;
    public event Action<Vector2Int[]> OnRevival;
    
    public IPowerUpExecution PowerUpExecution
    {
        get => _powerUpExecutionExecution;
        set
        {
            _powerUpExecutionExecution = value;
            powerUpInput.SetActive(value != null);
        }
    }
    
    public IEnumerator IEActivePowerUp(Vector3 position)
    {
        if (_powerUpExecutionExecution == null) yield break;
        var coordinate = GameManager.Instance.BoardEntity.InputCoordinateAvailable(position);
        if(coordinate == null) yield break;
        var copyPowerUp = _powerUpExecutionExecution;
        _powerUpExecutionExecution = null;
        yield return copyPowerUp.IEExecute(GameManager.Instance, coordinate.Value);
        powerUpInput.SetActive(false);
    }

    private IEnumerator IEPlayDestroyMiddleRow()
    {
        _powerUpExecutionExecution = new IGCRowDestroy();
        var coordinate = Vector2Int.one;
        var copyPowerUp = _powerUpExecutionExecution;
        _powerUpExecutionExecution = null;
        yield return copyPowerUp.IEExecute(GameManager.Instance, coordinate, false);
        powerUpInput.SetActive(false);
    }
    
    private IEnumerator IEPlayDestroyMiddleCol()
    {
        _powerUpExecutionExecution = new IGCColDestroy();
        var coordinate = Vector2Int.one;
        var copyPowerUp = _powerUpExecutionExecution;
        _powerUpExecutionExecution = null;
        yield return copyPowerUp.IEExecute(GameManager.Instance, coordinate, false);
        powerUpInput.SetActive(false);
    }

    public void DestroyRowAndCol()
    {
        StartCoroutine(IEPlayDestroyMiddleCol());
        StartCoroutine(IEPlayDestroyMiddleRow());
    }


    public void DestroyRandomTile(int tileAmount)
    {
        var tilesToDestroy = new List<Vector2Int>();
        var size = GameManager.Instance.BoardEntity.Size;
        int count = 0;
        while (tilesToDestroy.Count < tileAmount && count < 200)
        {
            count++;
            int x = Random.Range(0, size.x + 1);
            int y = Random.Range(0, size.y + 1);
            var tileCoord = new Vector2Int(x, y);
            if(tilesToDestroy.Contains(tileCoord)) continue;
            if(!GameManager.Instance.Board.HasRing(x, y)) continue;
            tilesToDestroy.Add(tileCoord);
        }
        OnRevival?.Invoke(tilesToDestroy.ToArray());
    }
}

