using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ColorChallenge : Challenge<ColorChallenge>, IChallenge
{
    private int _require;
    private Color _color;
    [SerializeField] private RingSize ringSizeImage = RingSize.MEDIUM_RING;
    [SerializeField] private Text text;
    [SerializeField] private GameObject tick;
    [SerializeField] private Image[] imageColors;

    public Color ColorOfChallenge => _color;

    public void Set(ColorData colorData)
    {
        _require = colorData.require;
        _color = colorData.color;
        imageColors[0].gameObject.SetActive(true);
        if (GameManager.Instance.ShowFullSkin)
        {
            imageColors[1].gameObject.SetActive(true);
            imageColors[2].gameObject.SetActive(true);
            imageColors[0].sprite = GameManager.Instance.GetSkin(_color, RingSize.SMALL_RING);
            imageColors[1].sprite = GameManager.Instance.GetSkin(_color, RingSize.MEDIUM_RING);
            imageColors[2].sprite = GameManager.Instance.GetSkin(_color, RingSize.BIG_RING);
        }
        else
        {
            imageColors[0].sprite = GameManager.Instance.GetSkin(_color, ringSizeImage);
            imageColors[1].gameObject.SetActive(false);
            imageColors[2].gameObject.SetActive(false);
        }
       
        text.text = _require.ToString(CultureInfo.CurrentCulture);
    }
 
    public void Check(int value, Color color)
    {
        if (_require <= 0) return;
        if (color == _color)
        {
            _require = Mathf.Max(0, _require - value);
            text.text = _require.ToString(CultureInfo.CurrentCulture);
            if (_require == 0) SetComplete();
        }
    }

    public bool IsComplete() => _require <= 0;

    public void SetComplete()
    {
        text.gameObject.SetActive(false);
        tick.SetActive(true);
    }
    

    [Serializable]
    public class ColorData : IChallengeData
    {
        public int require;
        public Color color;
    }
}