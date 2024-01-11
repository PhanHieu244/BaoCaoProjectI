using DG.Tweening;
using Puzzle.UI;

public class ButtonShop : ButtonClick {
    protected override void OnClick() {
        Hub.Show(Hub.Get<IapShop>(PopUpPath.POP_UP_WOOD__UI__PACK_SHOP, null).gameObject).Play();
    }
}