using System.Collections;
using DG.Tweening;
using Falcon.FalconAnalytics.Scripts.Enum;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class SkinRewardPopUp : MonoBehaviour, IPopUpContent
{
    [SerializeField] private Button backButton, claimButton;
    [SerializeField] private GameObject skinGroup;
    [SerializeField] private Image[] ringImages;
    [SerializeField] private Animator animatorShowReward;
    [SerializeField] private AnimationClip clip;
    [SerializeField] private Material grey;
    private bool _isClaim;
    private static readonly int UnpackReward = Animator.StringToHash("unpackReward");

    public void Setup(int randomRewardSkinID)
    {
        _isClaim = false;
        skinGroup.SetActive(false);
        backButton.onClick.AddListener(Back);
        claimButton.onClick.AddListener(() => OnShowReward(randomRewardSkinID));
    }
    
    private void Back()
    {
        var popup = Hub.Get<SkinRewardPopUp>(PopUpPath.POP_UP_WOOD__UI_REWARDSKIN);
        Hub.Hide(popup.gameObject).Play();
        AudioManager.Instance.PlaySound(EventSound.Click);
    }

    private void OnShowReward(int randomRewardSkinID)
    {
        if (_isClaim) return;
        AdsManager.Instance.ShowReward($"WIN_SKIN_REWARD", () =>
        {
            Data4Game.ResourceLog(GameDataManager.MaxLevelUnlock,FlowType.Source, "ads_skin_reward", $"adsFreeSkin{randomRewardSkinID}", "skin", 1);
            DevLog.Log("rewardSkin ID", randomRewardSkinID);
            GameDataManager.UnlockSkinByID(randomRewardSkinID);
            AudioManager.Instance.PlaySound(EventSound.Click);
            claimButton.image.material = grey;
            _isClaim = true;
            StartCoroutine(IEShowRewardSkin(randomRewardSkinID));
        });
    }

    private IEnumerator IEShowRewardSkin(int randomRewardSkinID)
    {
        GameDataManager.GetSkinByID(randomRewardSkinID).SetUpSkin(ringImages);
        animatorShowReward.SetBool(UnpackReward, true);
        yield return new WaitForSeconds(clip.length);
        animatorShowReward.gameObject.SetActive(false);
        skinGroup.SetActive(true);
        skinGroup.transform.localScale = new Vector3(.02f, .02f, .02f);
        yield return skinGroup.transform.DOScale(new Vector3(1.2f, 1.2f, 1f), .3f).SetEase(Ease.OutBack);
        yield return skinGroup.transform.DOScale(new Vector3(1f, 1f, 1f), .13f).SetEase(Ease.OutBack);
    }
}