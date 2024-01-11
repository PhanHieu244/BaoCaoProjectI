using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinTabButtons : MonoBehaviour
{
    [SerializeField] private Sprite unselectedButtonSprite;
    [SerializeField] private Sprite selectedButtonSprite;

    [SerializeField] private SkinTabButtonElement[] buttons;
    private SkinTabButtonElement selectedButton;

    private void Awake()
    {
        if (buttons == null || buttons.Length == 0) return;

        selectedButton = buttons[0];

        foreach (var button in buttons)
        {
            button.thisButton.onClick.AddListener(() => UpdateTabs(button));
        }    
    }

    private void OnEnable()
    {
        if (buttons == null || buttons.Length == 0) return;

        selectedButton = buttons[0];
        
        foreach (var button in buttons)
        {
            SkinButtonUnselected(button);
        }

        SkinButtonSelected(selectedButton);
    }

    private void UpdateTabs(SkinTabButtonElement newSelectedButton)
    {
        if (newSelectedButton == selectedButton) return;

        SkinButtonUnselected(selectedButton);
        selectedButton = newSelectedButton;
        SkinButtonSelected(selectedButton);
    }

    private void SkinButtonSelected(SkinTabButtonElement elementClicked)
    {
        if (elementClicked.skinTabBoard != null)
        {
            elementClicked.skinTabBoard.SetActive(true);
            elementClicked.thisImage.sprite = selectedButtonSprite;
        }
    }

    private void SkinButtonUnselected(SkinTabButtonElement elementClicked)
    {
        if (elementClicked.skinTabBoard != null)
        {
            elementClicked.skinTabBoard.SetActive(false);
            elementClicked.thisImage.sprite = unselectedButtonSprite;
        }
    }
}
