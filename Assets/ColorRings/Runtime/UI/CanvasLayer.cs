using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLayer : MonoBehaviour
{
    public void OnEnable()
    {
        var canvas = this.GetComponent<Canvas>();
        canvas.sortingLayerName = "UI";
    }
}
