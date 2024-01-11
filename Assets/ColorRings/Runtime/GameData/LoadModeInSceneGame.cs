using UnityEngine;

[RequireComponent(typeof(GameLoader))]
public class LoadModeInSceneGame : MonoBehaviour
{
#if UNITY_EDITOR
    private void Start()
    {
        GameLoader.Instance.Load(GameDataManager.MaxLevelUnlock);
    }
#endif
}
