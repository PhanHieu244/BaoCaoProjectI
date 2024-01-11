using System.Collections;
using UnityEngine;

public interface IDestroyExecute
{
    public IEnumerator Execute(GameManager gameManager, Vector2Int coordinate, bool isUpdateCount = true);
}                