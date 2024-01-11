using System;
using UnityEngine;

public class BrokenRingCounter : MonoBehaviour
{
    private int _ringBreakCount;
    private void Start()
    {
        GameManager.Instance.OnComboComplete += AddRingBreak;
        GameManager.Instance.OnWinGame += OnCountDone;
    }

    private void AddRingBreak(int ringBreakAmount)
    {
        _ringBreakCount += ringBreakAmount;
    }

    private void OnCountDone()
    {
        GameDataManager.RingBreakAdd += _ringBreakCount;
    }
    
}