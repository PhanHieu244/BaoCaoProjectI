using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGift : MonoBehaviour
{
    [SerializeField] private Sprite hammerGift;
    [SerializeField] private Sprite destroyRowGift;
    [SerializeField] private Sprite destroyColGift;
    [SerializeField] private Sprite swapGift;
    [SerializeField] private Image image;
    [SerializeField] private Text amountText;
    [SerializeField] private Canvas[] canvasLightBack;

    public void Setup(PowerUpType type, int amount)
    {
        image.sprite = GetSpriteByType(type);
        amountText.text = "x" + amount;

        var sizeLightBack = canvasLightBack.Length;
        for (var index = 0; index < sizeLightBack; index++)
        {
            canvasLightBack[index].sortingOrder = 2;
            canvasLightBack[index].sortingLayerName = "UI";
        }
        
    }

    private Sprite GetSpriteByType(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.Tile: return hammerGift;
            case PowerUpType.Row: return destroyRowGift;
            case PowerUpType.Line: return destroyColGift;
            case PowerUpType.Swap: return swapGift;
        }
        return hammerGift;
    }
}
