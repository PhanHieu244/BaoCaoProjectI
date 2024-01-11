using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_BackgroundToggle : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;
    private void Start()
    {
        gameObject.SetActive(!_toggle.isOn);
    }

    public void SetUnActive(bool on)
    {
        gameObject.SetActive(!on);
    }
}