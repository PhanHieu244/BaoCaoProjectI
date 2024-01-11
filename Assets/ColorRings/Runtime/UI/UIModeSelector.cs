using UnityEngine;
using UnityEngine.UI;

public class UIModeSelector : MonoBehaviour
{
    [SerializeField] private ModeGameType modeGameType = ModeGameType.Endless;
        
    private void Awake()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        AudioManager.Instance.PlaySound(EventSound.Click);
        GameLoader.Instance.Load(modeGameType);
    }
}
