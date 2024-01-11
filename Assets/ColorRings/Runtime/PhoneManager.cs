using System;
using UnityEngine;

public class PhoneManager : PersistentSingleton<PhoneManager>
{
    private bool _viberate = true;
    

    public void Viberate(long timeMilies)
    {
        if (!_viberate) return;
        Vibration.Vibrate(timeMilies);
    }

    public void SwitchOnOffViberate(bool state)
    {
        GameDataManager.VibrateState = state ? 1 : 0;
        _viberate = state;
    }

}
