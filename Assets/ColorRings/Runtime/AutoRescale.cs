using System;
using UnityEngine;
using UnityEngine.UI;


public class AutoRescale : MonoBehaviour
{
    [SerializeField] private Transform[] transforms;

    private void OnDisable()
    {
        foreach (var t in transforms)
        {
            t.localScale = Vector3.one;
        }
    }
}