using Puzzle.UI;
using UnityEngine;

public class ButtonQuitPopUp : ButtonClick {
    public GameObject popUp;
    protected override void OnClick() {
        Hub.Hide(popUp);
    }
}