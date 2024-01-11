using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class ButtonClick : MonoBehaviour {
    protected virtual void Awake() {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        button.onClick.AddListener(() => AudioManager.Instance.PlaySound(EventSound.Click));
#if UNITY_EDITOR
        button.onClick.AddListener(() => Debug.Log($"OnClick{GetType()}"));
#endif
    }

    protected abstract void OnClick();
    public virtual bool IsAvailable => true;
}