using System.Collections.Generic;
using UnityEngine;

namespace ColorRings.Runtime.Reward.RewardGift
{
    public class TestReward : MonoBehaviour
    {
        [SerializeReference, SubclassSelector] private IGift[] gifts;
        private List<IPresenter> presenterRewards = new List<IPresenter>();
        private void Start()
        {
            foreach (var gift in gifts)
            {
                var present = PresentPool.Instance.Get(gift.GetType());
                presenterRewards.Add(present);
                present.SetUpInfo(gift);
            }
        }

        /*public void Release()
        {
            foreach (var presenterReward in presenterRewards)
            {
                PresentGiftPool.Instance[]
            }
        }*/
    }
}