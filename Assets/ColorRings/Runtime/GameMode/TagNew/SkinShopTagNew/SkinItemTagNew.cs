using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkinItemTagNew : TagNewEntity
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
        PopUpBuySkin.OnBuySkin += Refresh;
        base.Start();
    }

    protected override void OnEnable()
    {
        StartCoroutine(IECheck());
    }

    private IEnumerator IECheck()
    {
        yield return null;
        yield return null;
        base.OnEnable();
    }
}
