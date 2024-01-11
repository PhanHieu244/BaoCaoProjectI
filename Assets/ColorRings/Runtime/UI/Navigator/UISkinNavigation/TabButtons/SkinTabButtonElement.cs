using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class SkinTabButtonElement : MonoBehaviour
{
    [SerializeField] public GameObject skinTabBoard;
    public Button thisButton;
    public Image thisImage;

    private void Awake()
    {
        thisButton = GetComponent<Button>();
        thisImage = GetComponent<Image>();
    }
}
