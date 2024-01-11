using System;
using ColorRings.Runtime.Board.Entity;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IBoardShapeInLevel
{
    BoardShape BoardShape { get; }
}

[Serializable]
public class BoardShapeAdventure : IBoardShapeInLevel
{
    [SerializeField] private BoardShape boardShape;

    public BoardShape BoardShape => boardShape;
}

[Serializable]
public class RandomBoardShapeAdventure : IBoardShapeInLevel
{
    [SerializeField] private BoardShape[] boardShapes;

    public BoardShape BoardShape => boardShapes[Random.Range(0, boardShapes.Length)];
}