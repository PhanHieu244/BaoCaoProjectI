using System.Collections;
using UnityEngine;

public class IGCColDestroy : InGamePowerUpContainer<InGamePowerUpConfig>
{
    [SerializeReference, SubclassSelector] private IDestroyExecute _destroyExecute = new ClassicColDestroy();
    public override IEnumerator IEExecute(GameManager gameManager, Vector2Int coordinate, bool isUpdateCount = true)
    {
        yield return _destroyExecute.Execute(gameManager, coordinate, isUpdateCount);
        yield return base.IEExecute(gameManager, coordinate, isUpdateCount);
    }
}


