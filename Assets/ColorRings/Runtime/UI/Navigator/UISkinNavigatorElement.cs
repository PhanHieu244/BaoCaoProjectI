using Puzzle.UI;
using UnityEngine;

namespace ColorRings.Runtime.UI.Navigator
{
    public class UISkinNavigatorElement: NavigatorElement<UISkinNavigation>
    {
        public override GameObject GetContent()
        {
            var popup = Hub.Get<UISkinNavigation>(PopUpPath.POP_UP_WOOD__UI_SKIN_NAVIGATION);
            return popup.gameObject;
        }
    }
}