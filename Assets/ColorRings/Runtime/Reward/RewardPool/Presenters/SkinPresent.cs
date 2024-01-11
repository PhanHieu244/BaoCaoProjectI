using UnityEngine;
using UnityEngine.UI;

public class SkinPresent : MonoPresenter<SkinGift>
{
    [SerializeField] private Image[] ringImages;
    private Skin _skin;

    public override void SetUpInfo(SkinGift reward)
    {
        _skin = GameDataManager.GetSkinByID(reward.ID);
        _skin.SetUpSkin(ringImages);
    }
}