using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgeSlider : MonoBehaviour
{
    private Slider _ageSlider;
    [SerializeField] private Vector2Int ageRange;
    [SerializeField] private Text tAge;
    private int _age;

    private void Awake()
    {
        _ageSlider = GetComponent<Slider>();
        _ageSlider.onValueChanged.AddListener(UpdateText);
        UpdateText(0);
        if (ageRange.x < 0) ageRange.x = 0;
        if (ageRange.y < ageRange.x) ageRange.y = ageRange.x;
    }

    private void UpdateText(float value)
    {
        _age = (int)(value * (ageRange.y - ageRange.x) + ageRange.x);
        tAge.text = "Your age is: " + _age;
    }

    public int GetAge() => _age;
}
