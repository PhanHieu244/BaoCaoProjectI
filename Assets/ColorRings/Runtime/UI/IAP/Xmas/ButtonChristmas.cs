using Puzzle.UI;
using UnityEngine;

public class ButtonChristmas : ButtonClick
{
    [SerializeField] private RemainTimeDisplay remainTimeDisplay;

    protected override void Awake()
    {
        remainTimeDisplay.OnExpireTime += OnExpireTime;
        base.Awake();
    }

    private void OnExpireTime()
    {
        gameObject.SetActive(false);
        remainTimeDisplay.OnExpireTime -= OnExpireTime;
    }

    protected override void OnClick() {
        Hub.Show(Hub.Get<Christmas>(PopUpPath.POP_UP_WOOD__UI__PACK_XMAS, null).gameObject);
    }

    public override bool IsAvailable {
        get => new ChristmasIapPack().Available;
    }
}

