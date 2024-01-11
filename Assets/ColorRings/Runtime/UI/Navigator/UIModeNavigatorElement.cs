using ColorRings.Runtime.UI.Navigator.ModeNavigation;
using Puzzle.UI;
using UnityEngine;

namespace ColorRings.Runtime.UI.Navigator
{
    public class UIModeNavigatorElement: NavigatorElement<UIModeNavigation>
    {
        public override GameObject GetContent()
        {
            var popup = Hub.Get<UIModeNavigation>(PopUpPath.POP_UP_WOOD__UI_MODE_NAVIGATION);
            return popup.gameObject;
        }
    }
}