using System.Threading.Tasks;
using CodeStage.AntiCheat.Common;
using ColorRings.Runtime.GameData;
using UnityEngine;

[DefaultExecutionOrder(999)]
public class DataInitor : PersistentSingleton<DataInitor>
{
    private static bool _isInit = false;
    
    protected override void Awake()
    {
        base.Awake();
        if (_isInit) return;
        LoadData();
        _isInit = true;
    }
    
    private async void LoadData()
    {
        //InitData
        UnityApiResultsHolder.InitForAsyncUsage(true);
        await Task.Run(DataHandle.InitData);
    }
}
