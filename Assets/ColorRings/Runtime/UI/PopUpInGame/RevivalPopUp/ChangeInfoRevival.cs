using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ChangeInfoRevival : MonoBehaviour
{
    private Text _infoText;
    
    private void Awake()
    {
        _infoText = GetComponent<Text>();
    }

    private void OnEnable()
    {
        _infoText.text = GetTextInfo;
    }

    private static string GetTextInfo => GameManager.Instance.Level.ModeGameType switch
    {
        ModeGameType.Normal => "Destroy 4 spaces to keep playing",
        ModeGameType.Bonus => "More space more time",
        _ => "Destroy 4 spaces to keep playing"
    };
}