using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkinTabBarTagNew : TagNewEntity
{ 
    [SerializeField] private Button deActiveBut;
    [SerializeField] private Button[] refreshBut;

    private void Awake()
    {
        deActiveBut.onClick.AddListener(UnActiveTag);
        foreach (var rButton in refreshBut)
        {
            rButton.onClick.AddListener(Refresh);
        }
    }

    protected override void Start()
    {
        PopUpBuySkin.OnBuySkin += OnRefresh;
        GameDataManager.OnUnlockSkin += OnRefresh;
        SkinItem.OnSelect += OnRefresh;
        base.Start();
    }

    private void OnRefresh()
    {
        if (!isShowTag) return;
        StartCoroutine(IEOnRefresh());
    }
    
    private void OnRefresh(int i)
    {
        if (!isShowTag) return;
        StartCoroutine(IEOnRefresh());
    }

    private IEnumerator IEOnRefresh()
    {
        yield return new WaitForSeconds(0.12f);
        Refresh();
    }
    
    protected override void OnDestroy()
    {
        GameDataManager.OnUnlockSkin -= OnRefresh;
        base.OnDestroy();
    }
}
