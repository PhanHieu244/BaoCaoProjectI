using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ModeBarTagNew : TagNewEntity
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
}