using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class ResolutionScaler : MonoBehaviour
{
    private void Awake()
    {
        var ratio =  Screen.width / (float)Screen.height;
        var scaler = GetComponent<CanvasScaler>();
        var f = scaler.referenceResolution.x / scaler.referenceResolution.y;
        scaler.matchWidthOrHeight = ratio > f  ? 1 : 0;
    }
}
