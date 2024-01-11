using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ColorInfoChallenge : MonoBehaviour
{
    private int _require;
    private Color _color;
    [SerializeField] private RingSize ringSizeImage = RingSize.BIG_RING;
    [SerializeField] private Text text;
    [SerializeField] private Image[] imageColors;

    public Color ColorOfChallenge => _color;

    public void Set(ColorChallenge.ColorData colorData, Skin skin)
    {
        _require = colorData.require;
        _color = colorData.color;
        imageColors[0].gameObject.SetActive(true);
        SetUpVisualChallenge(skin);
    }

    public void SetUpVisualChallenge(Skin skin)
    {
        if (skin.ShowFullSkin)
        {
            imageColors[1].gameObject.SetActive(true);
            imageColors[2].gameObject.SetActive(true);
            imageColors[0].sprite = skin[_color, RingSize.SMALL_RING];
            imageColors[1].sprite = skin[_color, RingSize.MEDIUM_RING];
            imageColors[2].sprite = skin[_color, RingSize.BIG_RING];
        }
        else
        {
            imageColors[0].sprite = skin[_color, ringSizeImage];
            imageColors[1].gameObject.SetActive(false);
            imageColors[2].gameObject.SetActive(false);
        }
        text.text = _require.ToString(CultureInfo.CurrentCulture);
    }
}