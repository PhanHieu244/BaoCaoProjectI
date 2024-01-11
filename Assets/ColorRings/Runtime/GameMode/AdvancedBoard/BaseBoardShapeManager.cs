using ColorRings.Runtime.Board.Entity;
using UnityEngine;

public abstract class BaseBoardShapeManager : MonoBehaviour
{
    public abstract BoardShape GetBoardShape();
    public abstract bool HasLock();
}