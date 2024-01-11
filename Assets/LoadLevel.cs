using UnityEngine;

public class LoadLevel : GameLoader
{
    public int levelSelect;

    public override void Load(int level = -1, GameObject popup = null)
    {
        base.Load(levelSelect - 1, popup);
    }
}
