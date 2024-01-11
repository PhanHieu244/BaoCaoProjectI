using ColorRings.Runtime.Board.Entity;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/CustomBoardLevel", fileName = "Level 1", order = 0)]
public class CustomBoardLevel : Level
{
    [field: SerializeField] public BoardShape BoardShape { get; private set; }
}