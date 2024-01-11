using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GameLoaderTestLevel : GameLoader
{
    [SerializeField] private Level customLevel;

#if UNITY_EDITOR
    [Button]
    private void LoadCustomLevel()
    {

    }
#endif
}
