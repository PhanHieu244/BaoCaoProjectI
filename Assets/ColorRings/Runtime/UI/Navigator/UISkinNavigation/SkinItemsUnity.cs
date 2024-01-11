using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkinItemsUnity
{
    public Action onDataChange;
    public Action<int, int> onBuy;
    public Action<string, Sprite[]> onShowInfo;
    private Color[] _colorSkinItems;
    private readonly int _skinAmount;
    private int _colorsRange;

    public SkinItemsUnity()
    {
        _skinAmount = GameDataManager.SkinAmount;
        InitColorData();
    }

    public void ShowPreview()
    {
        var idSkin = GameDataManager.CurrentSkin;
        var skin = GameDataManager.GetSkinByID(idSkin, true);
        var color = GetColorByID(idSkin);
        Sprite[] ringsImageShowPreview = new Sprite[3];
        ringsImageShowPreview[(int)RingSize.SMALL_RING] = skin[color, RingSize.SMALL_RING];  
        ringsImageShowPreview[(int)RingSize.MEDIUM_RING]= skin[color, RingSize.MEDIUM_RING];  
        ringsImageShowPreview[(int)RingSize.BIG_RING] = skin[color, RingSize.BIG_RING];  
        onShowInfo?.Invoke(SkinItem.SelectText, ringsImageShowPreview);
    }

    private void InitColorData()
    {
        _colorsRange = (int) Color.COUNT;
        _colorSkinItems = new Color[_skinAmount];
        int count = 0;
        int maxCount = 2;
        var currentColor = Color.NONE;
        for (int i = 0; i < _skinAmount; i++)
        {
            _colorSkinItems[i] = GetRandomColor();
            if (currentColor == _colorSkinItems[i]) count++;
            else count = 0;
            if (count > maxCount) i--;
        }
    }
    

    private Color GetRandomColor()
    {
        return (Color) Random.Range(1, _colorsRange);
    }

    public Color GetColorByID(int id) => _colorSkinItems[id];
}