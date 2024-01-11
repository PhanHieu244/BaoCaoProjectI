using System.Collections;
using UnityEngine;

public class IGCRowDestroy : InGamePowerUpContainer<InGamePowerUpConfig>
{
    [SerializeReference, SubclassSelector] private IDestroyExecute _destroyExecute = new ClassicRowDestroy();
    public override IEnumerator IEExecute(GameManager gameManager, Vector2Int coordinate, bool isUpdateCount = true)
    {
        yield return _destroyExecute.Execute(gameManager, coordinate, isUpdateCount);
        yield return base.IEExecute(gameManager, coordinate, isUpdateCount);
    }
}