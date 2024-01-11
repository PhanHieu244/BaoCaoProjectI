using System.Collections;
using DG.Tweening;
using Puzzle.UI;
using UnityEngine;

namespace ColorRings.Runtime.UI.PopUpInGame
{
    public class SkinStreakWinBar : SkinStreakBar
    {
        [SerializeField] private float duration = 1f;
        [SerializeField] private Animator animator;
        [SerializeField] private AnimationClip clip;
        [SerializeField] private BlockUI blockUI;
        private int _randomRewardSkinID;
        private static readonly int Active = Animator.StringToHash("active");

        protected override void OnEnable()
        {
            blockUI.Block();
            base.OnEnable();
        }

        protected override void SetUpSkinBar(int crtStreak)
        {
            base.SetUpSkinBar(crtStreak);
            var isRemainRewardSkin = GameDataManager.IsRemainRewardSkin(out _randomRewardSkinID);
            if (!isRemainRewardSkin)
            {
                blockUI.UnBlock();
                return;
            }
            StartCoroutine(IEIncreaseSkinStreak(GameDataManager.IncreaseSkinStreak()));
            
        }
        
        private IEnumerator IEIncreaseSkinStreak(int streak)
        {
            var isRewardStreak = streak >= GameConst.MaxSkinStreak;
            yield return new WaitForSeconds(0.3f);

            while (slider.value < leftPointValues[streak - 1])
            {
                slider.value += duration * Time.deltaTime;
                yield return null;
            }
            if (isRewardStreak)
            {
                yield return IEShowRewardAnim();
                yield return new WaitForSeconds(2f);
                blockUI.UnBlock();
                yield break;
            }
            while (activePoints[streak - 1].fillAmount < 1)
            {
                activePoints[streak - 1].fillAmount += duration * Time.deltaTime;
                yield return null;
            }
            blockUI.UnBlock();
        }
        
        
        private IEnumerator IEShowRewardAnim()
        {
            animator.SetBool(Active, true);
            yield return DOVirtual.DelayedCall(clip.length + 1f, () =>
            {
                var popup = Hub.Get<SkinRewardPopUp>(PopUpPath.POP_UP_WOOD__UI_REWARDSKIN);
                Hub.Show(popup.gameObject).Play();
                popup.Setup(_randomRewardSkinID);
            });
        }
        
    }
}