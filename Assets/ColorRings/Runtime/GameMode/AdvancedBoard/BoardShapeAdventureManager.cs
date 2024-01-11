using System;
using ColorRings.Runtime.Board.Entity;
using NaughtyAttributes;
using UnityEngine;

public class BoardShapeAdventureManager : BaseBoardShapeManager
{
    [SerializeField] private BoardShape classicBoard;
    [SerializeField] private bool hasLock = true;

    private void Awake()
    {
        Debug.Log("----------------------------------" +hasLock);
    }

    public override BoardShape GetBoardShape()
    {
        var boardShape = GameManager.Instance.Level.BoardShape;
        if (boardShape is null) return classicBoard;
        return boardShape;
    }

    public override bool HasLock() => hasLock;

    [Button]
    private void Log()
    {
        Debug.Log(hasLock);
    }
}