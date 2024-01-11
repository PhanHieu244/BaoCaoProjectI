using Puzzle.UI;

public class ButtonNoAds : ButtonClick {
    protected override void OnClick() {
        Hub.Show(Hub.Get<NoAds>(PopUpPath.POP_UP_WOOD__UI__PACK_NOADS, null).gameObject);
    }

    public override bool IsAvailable {
        get => IAPSubscription.Instance != null && IAPSubscription.Instance.IsInitialized();
    }
}