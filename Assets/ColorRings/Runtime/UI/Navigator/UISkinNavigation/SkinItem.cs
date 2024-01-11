using System;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkinItem : MonoBehaviour
{
    [SerializeField] private Image[] ringsImage;
    [SerializeField] private Text coinText;
    [SerializeField] private Button bBuy, bShowInfo, bSelect;
    [SerializeField] private GameObject selected,coin, blockBuy;

    [Header("Skin Tier Frame")]
    [SerializeField] private SkinTiersFrame skinFrameSpritesData;
    [SerializeField] private Image frameImage;

    public static event Action OnSelect;
    private const string UnlockText = "Unlocked";
    public static readonly string SelectText = "Selected";
    private int _idSkin;
    private IBuyCondition _buyCondition;
    private SkinItemsUnity _skinItemsUnity;
    private Skin _skin;
    private Color _color;
    
    public void Init(int id, SkinItemsUnity skinItemsUnity)
    {
        _idSkin = id;
        _buyCondition = GameDataManager.SkinShopData.GetConditionByID(id);
        _skinItemsUnity = skinItemsUnity;
        skinItemsUnity.onDataChange -= SetUpGroupUnity;
        skinItemsUnity.onDataChange += SetUpGroupUnity;
        SetUp();
      
    }

    private void OnEnable()
    {
        bBuy.onClick.RemoveListener(Buy);
        bBuy.onClick.AddListener(Buy);
        bShowInfo.onClick.RemoveListener(OnClickShowInfo);
        bShowInfo.onClick.AddListener(OnClickShowInfo);
        bSelect.onClick.RemoveListener(SelectSkin);
        bSelect.onClick.AddListener(SelectSkin);
    }

    private void OnDisable()
    {
        bBuy.onClick.RemoveListener(Buy);
        bShowInfo.onClick.RemoveListener(ShowInfo);
    }

    private void SetUp()
    {
        SetUpRingsImage();
        SetUpGroupUnity();
        if (_buyCondition is null)
        {
            coin.gameObject.SetActive(false);
            return;
        }
        coinText.text = _buyCondition.Coin.ToString();
    }

    private void SetUpRingsImage()
    {
        _skin = GameDataManager.GetSkinByID(_idSkin, true);
        _color = _skinItemsUnity.GetColorByID(_idSkin);
        ringsImage[(int)RingSize.SMALL_RING].sprite = _skin[_color, RingSize.SMALL_RING];  
        ringsImage[(int)RingSize.MEDIUM_RING].sprite = _skin[_color, RingSize.MEDIUM_RING];  
        ringsImage[(int)RingSize.BIG_RING].sprite = _skin[_color, RingSize.BIG_RING];  
    }

    private void SetUpGroupUnity()
    {
        SetupTierFrame();
        bBuy.gameObject.SetActive(false);
        bSelect.gameObject.SetActive(false);
        selected.SetActive(false);
        coin.SetActive(false);
        blockBuy.SetActive(false);
        if (_idSkin == GameDataManager.CurrentSkin)
        {
            selected.gameObject.SetActive(true);
            return;
        }
        if (GameDataManager.SkinAvailable(_idSkin))
        {
            bSelect.gameObject.SetActive(true);
            return;
        }
        coin.SetActive(true);
        if (GameDataManager.Coin < _buyCondition.Coin || !_buyCondition.IsAvailable())
        {
            blockBuy.SetActive(true);
            return;
        }
        bBuy.gameObject.SetActive(true);
    }

    private void SetupTierFrame()
    {
        if (frameImage == null) return;
        if (skinFrameSpritesData == null)
        {
            frameImage.gameObject.SetActive(false);
            return;
        }

        Sprite frameSprite = null;

        if (_idSkin == GameDataManager.CurrentSkin)
        {
            frameSprite = skinFrameSpritesData.GetSprite(_buyCondition.SkinTier, selected==true);
        }
        else
        {
            frameSprite = skinFrameSpritesData.GetSprite(_buyCondition.SkinTier, selected==false);
        }
        
        if (frameSprite == null)
        {
            frameImage.gameObject.SetActive(false);
            return;
        }

        frameImage.sprite = frameSprite;
        frameImage.gameObject.SetActive(true);
    }

    private void Buy()
    {
        AudioManager.Instance.PlaySound(EventSound.Click);
        ShowInfo();
        if (GameDataManager.Coin < _buyCondition.Coin) return;
        _skinItemsUnity.onBuy.Invoke(_idSkin, _buyCondition.Coin);
    }

    private void OnClickShowInfo()
    {
        AudioManager.Instance.PlaySound(EventSound.Click);
        ShowInfo();
    }

    private void ShowInfo()
    {
        Sprite[] ringsImageShowPreview = new Sprite[3];
        ringsImageShowPreview[(int)RingSize.SMALL_RING] = _skin[_color, RingSize.SMALL_RING];  
        ringsImageShowPreview[(int)RingSize.MEDIUM_RING]= _skin[_color, RingSize.MEDIUM_RING];  
        ringsImageShowPreview[(int)RingSize.BIG_RING] = _skin[_color, RingSize.BIG_RING]; 
        _skinItemsUnity.onShowInfo.Invoke( GetInfoText(), ringsImageShowPreview);
    }

    private string GetInfoText()
    {
        if (GameDataManager.SkinAvailable(_idSkin))
        {
            return _idSkin == GameDataManager.CurrentSkin ? SelectText : UnlockText;
        }
        return _buyCondition.GetSkinText();
    }
    
    public int GetSkinId()
    {
        return _idSkin;
    }

    private void SelectSkin()
    {
        AudioManager.Instance.PlaySound(EventSound.Click);
        if(!GameDataManager.SkinAvailable(_idSkin)) return;
        GameDataManager.SelectSkin(_idSkin);
        selected.SetActive(true);
        bSelect.gameObject.SetActive(false);
        _skinItemsUnity.onDataChange.Invoke();
        OnSelect?.Invoke();
        ShowInfo();
    }

    private void OnDestroy()
    {
        OnSelect = null;
    }
}