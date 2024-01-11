using Puzzle.UI;

public class ButtonBeginner : ButtonClick {
    public override bool IsAvailable {
        get => new BeginnerIapPack().Available;
    }

    protected override void OnClick() {
        Hub.Show(Hub.Get<Beginner>(PopUpPath.POP_UP_WOOD__UI__PACK_BEGINNER, null).gameObject);
    }
}