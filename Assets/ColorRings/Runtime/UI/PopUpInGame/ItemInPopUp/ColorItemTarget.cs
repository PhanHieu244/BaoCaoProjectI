using UnityEngine;
using UnityEngine.UI;

public class ColorItemTarget : MonoBehaviour
{
    [SerializeField] private RingSize ringSizeImage = RingSize.BIG_RING;
    [SerializeField] private Image[] imageColors;
    [SerializeField] private GameObject check, uncheck;

    public void SetUp(Color color, bool challengeIsComplete)
    {
        if (GameManager.Instance.ShowFullSkin)
        {
            imageColors[1].gameObject.SetActive(true);
            imageColors[2].gameObject.SetActive(true);
            imageColors[0].sprite = GameManager.Instance.GetSkin(color, RingSize.SMALL_RING);
            imageColors[1].sprite = GameManager.Instance.GetSkin(color, RingSize.MEDIUM_RING);
            imageColors[2].sprite = GameManager.Instance.GetSkin(color, RingSize.BIG_RING);
        }
        else
        {
            imageColors[0].sprite = GameManager.Instance.GetSkin(color, ringSizeImage);
            imageColors[1].gameObject.SetActive(false);
            imageColors[2].gameObject.SetActive(false);
        }
        check.SetActive(challengeIsComplete);
        uncheck.SetActive(!challengeIsComplete);
    }
}