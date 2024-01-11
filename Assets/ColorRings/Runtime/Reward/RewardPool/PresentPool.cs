using UnityEngine;

[DefaultExecutionOrder(999)]
public class PresentPool : RewardPool<IGift, IPresenter, PresenterAttribute, PresentPool>
{
    protected override void Awake()
    {
        base.Awake();
        Initialization();
    }
}