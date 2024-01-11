using DG.Tweening;
using Puzzle.UI;
using UnityEngine;

public class GameInitialized : MonoBehaviour
{
    private void Awake()
    {
        DOTween.Init();
    }
}
