using System;
using CodeStage.AntiCheat.Storage;
using ColorRings.Runtime.GameData;

[Serializable]
public class SkinTagNewStrategy : ITagNewStrategy
{
    public TagNewType TagNewType => TagNewType.ModeSkin;

    public void Checked(string id)
    {
        int _id = int.Parse(id);
        SkinStatus.Instance.OnSelectSkin(_id);
        SkinStatus.Instance[_id] = 1; 
    }

    public bool HasNew(string id)
    {
        int _id = int.Parse(id);

        return GameDataManager.SkinAvailable(_id) && SkinStatus.Instance[_id] == 0;
    }
}

[Serializable]
public class SkinTabBarTagNewStrategy : ITagNewStrategy
{
    public TagNewType TagNewType => TagNewType.ModeSkinTab;

    public void Checked(string id)
    {
        SkinStatus.Instance.OnCheckNewSkin();
    }

    public bool HasNew(string id)
    {
        return SkinStatus.Instance.HasNewSkin();
    }
}

public class Singleton<T> where T : Singleton<T>, new()
{
    public static T Instance { get; private set; } = new T();
}

public class SkinStatus : Singleton<SkinStatus> {
    
    private WrapperArray<int> status;
    public SkinStatus() {
        status = DataHandle.GetData<WrapperArray<int>>("skin_status");
        status.values[0] = 1;
        GameDataManager.OnUnlockSkin += OnUnlockNewSkin;
        if (status.values.Length >= GameDataManager.SkinAmount) 
            return;

        Array.Resize(ref status.values, GameDataManager.SkinAmount);
        status.values[GameDataManager.CurrentSkin] = 1;
        DataHandle.Save("skin_status", status);
    }

    public int this[int index]
    {
        get
        {
            if (index >= GameDataManager.SkinAmount) return 0;
            return status.values[index];
        }
        set
        {
            status.values[index] = value;
            DataHandle.Save("skin_status", status);
        }

    }

    public bool HasNewSkin()
    {
        return ObscuredPrefs.Get("hasNewSkin", 0) > 0;
    }

    public void OnCheckNewSkin()
    {
        ObscuredPrefs.Set("hasNewSkin", 0);
    }

    public void OnSelectSkin(int id)
    {
        if (status.values[id] == 1) return;
        var newSkinCount = ObscuredPrefs.Get("hasNewSkin", 0);
        if (newSkinCount <= 0) return;
        ObscuredPrefs.Set("hasNewSkin", newSkinCount - 1);
    }

    private void OnUnlockNewSkin(int id)
    {
        var newSkinCount = ObscuredPrefs.Get("hasNewSkin", 0);
        ObscuredPrefs.Set("hasNewSkin", newSkinCount + 1);
    }

    public void OnCheckAllOwnedSkin()
    {
        DevLog.Log("Owned tab disabled");
        for (int id = 0; id < status.values.Length; id++)
        {
            if (GameDataManager.SkinAvailable(id)) status.values[id] = 1;
        }
    }
}

[Serializable]
public class WrapperArray<T> : IData
{
    public T[] values;

    public WrapperArray() { 
        values = new T[1]; 
    }
}
