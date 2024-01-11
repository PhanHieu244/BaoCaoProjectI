using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.Detectors;
using CodeStage.AntiCheat.Storage;
using UnityEngine;

public class CheatDetector : PersistentSingleton<CheatDetector>
{
    [SerializeField] private GameObject warningPopup;
    private void Start()
    {
        warningPopup.SetActive(false);
        ObscuredPrefs.NotGenuineDataDetected += OnCheatingDetected;
        ObscuredPrefs.DataFromAnotherDeviceDetected += OnCheatingDetected;
    }

    [ContextMenu("cheat")]
    private void OnCheatingDetected()
    {
        GameDataManager.DeleteAll();
        warningPopup.SetActive(true);
    }
}
