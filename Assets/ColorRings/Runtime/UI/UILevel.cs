using System.Globalization;
using DG.Tweening;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class UILevel : MonoBehaviour
{
    private void Awake()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private static void OnClick()
    {
        AudioManager.Instance.PlaySound(EventSound.Click);
        GameLoader.Instance.Load(GameDataManager.MaxLevelUnlock);
    }
}