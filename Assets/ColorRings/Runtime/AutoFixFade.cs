using UnityEngine;
using UnityEngine.UI;

public class AutoFixFade : MonoBehaviour
{
    [SerializeField] private Image[] _images;
    private UnityEngine.Color[] colors;


    private void Awake()
    {
        colors = new UnityEngine.Color[_images.Length];
        for (var index = 0; index < _images.Length; index++)
        {
            colors[index] = _images[index].color;
        }
    }

    private void OnDisable()
    {
        for (var index = 0; index < _images.Length; index++)
        {
            _images[index].color = colors[index];
        }
    }
}