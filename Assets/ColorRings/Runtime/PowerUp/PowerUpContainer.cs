using System;
using System.Collections;
using UnityEngine;

public abstract class PowerUpContainer<T> : MonoBehaviour where T : IPowerUpConfig
{
    [SerializeField] protected T powerUpConfig;
    [SerializeField] protected PowerUpData powerUpData;
}

public interface IPowerUpConfig { }
public interface IPowerUpExecution
{
    IEnumerator IEExecute(GameManager gameManager, Vector2Int coordinate, bool isUpdateCount = true);
}