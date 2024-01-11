using DG.Tweening;
using Puzzle.UI;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Screen = UnityEngine.Screen;

public class Navigator : MonoBehaviour, IValidator
{
    public INavigatorElement CurrentElement;

    [SerializeField] private UIHomeNavigatorElement homeNavigatorElement;
    public Ease ease;
    public float duration = 0.2f;

    private void Start()
    {
        ShowDefaultContent();
    }

    public void ShowDefaultContent()
    {
        CurrentElement = homeNavigatorElement;
        CurrentElement.OnActive().Play().Complete();
        var content = CurrentElement.GetContent();
        if (content != null) Hub.Show(content);
    }

#if UNITY_EDITOR
    public void Validate()
    {
        homeNavigatorElement = GetComponentInChildren<UIHomeNavigatorElement>();
    }
#endif
}

public interface INavigatorElement
{
    int index { get; }
    Tween OnActive();
    Tween OnDeActive();

    GameObject GetContent();
}


[RequireComponent(typeof(LayoutElement))]
public abstract class NavigatorElement<T> : ContentButtonClick<T>, INavigatorElement, IValidator where T : Component
{
    [SerializeField, ReadOnly] private Navigator navigator;
    [SerializeField, ReadOnly] private LayoutElement layoutElement;
    [SerializeField] private GameObject textGameObject;
    [SerializeField] private RectTransform transformIconBG;
    [SerializeField] private RectTransform transformIcon;
    [SerializeField] private Image bgImage;
    [SerializeField] private Sprite activeButtonSprite;
    [SerializeField] private Sprite unactiveButtonSprite;
    [SerializeField] private Sprite activeFrameSprite;
    [SerializeField] private Sprite unactiveFrameSprite;
    [field: SerializeField] public int index { get; private set; }

    private float DURATION => navigator.duration;
    //private const float DURATION = .15f;
    private const float UN_ACTIVE_WIDTH = 340;
    private const float ACTIVE_WIDTH = 400;
    private static readonly Vector2 ACTIVE_SIZE = new(179, 147);
    private static readonly Vector2 UN_ACTIVE_SIZE = new(179, 149);
    private static readonly Vector2 ACTIVE_FRAME_POSITION = new(0, 80);
    private static readonly Vector2 UN_ACTIVE_FRAME_POSITION = Vector2.zero;
    private static readonly Vector2 ACTIVE_ICON_POSITION = new (0, 10);
    private static readonly Vector2 UN_ACTIVE_ICON_POSITION = Vector2.zero;

#if UNITY_EDITOR
    public void Validate()
    {
        navigator = GetComponentInParent<Navigator>();
        layoutElement = GetComponent<LayoutElement>();
    }
#endif


    protected override void OnClickAndChecked()
    {
        if (!Available) return;
        if (ReferenceEquals(navigator.CurrentElement, this)) return;
        OnClick();
    }

    protected override void OnClick()
    {
        navigator.DOComplete(true);
        AudioManager.Instance.PlaySound(EventSound.Click);
        var navigatorElementTransition = DOTween.Sequence();
        var popUpTransition = DOTween.Sequence();

        var previousElement = navigator.CurrentElement;
        if (previousElement != null)
        {
            navigatorElementTransition.Join(previousElement.OnDeActive());
            popUpTransition.Append(Hub.Hide(previousElement.GetContent()));
        }
        
        navigator.CurrentElement = this;
        navigatorElementTransition.Join(OnActive());
        popUpTransition.Append(Hub.Show(GetContent()));

        DOTween.Sequence().SetTarget(navigator)
            .Join(navigatorElementTransition)
            .Join(popUpTransition).Play();
    }

    public Tween OnActive()
    {
        return DOTween.Sequence()
            .AppendCallback(() => bgImage.sprite = activeButtonSprite)
            .AppendCallback(() => transformIconBG.GetComponent<Image>().sprite = activeFrameSprite)
            .AppendCallback(() => textGameObject.SetActive(true))
            .AppendCallback(() => textGameObject.transform.localScale = Vector3.zero)
            .Join(transformIconBG.DOAnchorPos(ACTIVE_FRAME_POSITION, DURATION))
            .Join(transformIconBG.DOSizeDelta(ACTIVE_SIZE, DURATION))
            .Join(transformIcon.DOAnchorPos(ACTIVE_ICON_POSITION, DURATION))
            .Join(DOTween.To(() => UN_ACTIVE_WIDTH, f => layoutElement.minWidth = f, ACTIVE_WIDTH, DURATION))
            .Join(textGameObject.transform.DOScale(Vector3.one, DURATION).From(Vector3.zero));
    }

    public Tween OnDeActive()
    {
        return DOTween.Sequence()
            .AppendCallback(() => bgImage.sprite = unactiveButtonSprite)
            .AppendCallback(() => transformIconBG.GetComponent<Image>().sprite = unactiveFrameSprite)
            .AppendCallback(() => textGameObject.transform.localScale = Vector3.one)
            .Join(transformIconBG.DOAnchorPos(UN_ACTIVE_FRAME_POSITION, DURATION))
            .Join(transformIconBG.DOSizeDelta(UN_ACTIVE_SIZE, DURATION))
            .Join(transformIcon.DOAnchorPos(UN_ACTIVE_ICON_POSITION, DURATION))
            .Join(DOTween.To(() => ACTIVE_WIDTH, f => layoutElement.minWidth = f, UN_ACTIVE_WIDTH, DURATION))
            .Join(textGameObject.transform.DOScale(Vector3.zero, DURATION).From(Vector3.one))
            .AppendCallback(() => textGameObject.SetActive(false));
    }

    public abstract GameObject GetContent();
}

[RequireComponent(typeof(Button))]
public class ContentButtonClick<T> : MonoBehaviour where T : Component
{
    protected void Awake()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClickAndChecked);
    }

    protected virtual bool Available => true;


    protected virtual void OnClickAndChecked()
    {
        if (!Available) return;
        OnClick();
    }

    protected virtual void OnClick()
    {
        //        var content = Hub.GetContent<T>();
        //#if UNITY_EDITOR
        //        if (content == null)
        //        {
        //            throw new NullReferenceException(typeof(T) + "is not exist");
        //        }
        //#endif
        //        ContentManager.Show(content.gameObject).Play();
    }
}

