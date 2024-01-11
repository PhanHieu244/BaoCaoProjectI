using Puzzle.UI;
using UnityEngine;

public class UIHomeNavigatorElement : NavigatorElement<UIHomeNavigation>
{
    public override GameObject GetContent()
    {
        var popup = Hub.Get<UIHomeNavigation>(PopUpPath.POP_UP_WOOD__UI_HOME_NAVIGATION);
        return popup.gameObject;
    }
}