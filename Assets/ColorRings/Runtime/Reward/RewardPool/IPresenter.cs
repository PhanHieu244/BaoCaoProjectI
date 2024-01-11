using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public interface IPresenter
{
    Transform Transform { get; }
    void SetUpInfo(object reward);
    void Present(Transform parent = null);
    void Present(Transform parent, Vector3 localPos, float scale);
    public Tween OnShow(float duration = 0.2f, float targetScale = 1f);
    public Tween OnHide(float duration = 0.2f);
    void Release();
}

public interface IPresenter<in T> : IPresenter where T : IGift
{
    void SetUpInfo(T reward);

    void IPresenter.SetUpInfo(object reward)
    {
        if (reward is T rewardForward)
            SetUpInfo(rewardForward);
        else
            Debug.LogError($"{GetType()} is not presenter for {reward.GetType()}");
    }
}

public abstract class MonoPresenter<T> : MonoBehaviour, IPresenter<T> where T : IGift
{
    public Transform Transform => transform;
    public abstract void SetUpInfo(T reward);
    public void Release()
    {
        gameObject.SetActive(false);
        if (PresentPool.Instance is null) return;
        PresentPool.Instance.Release<T>(this);
        transform.SetParent(PresentPool.Instance.transform);
    }
    public virtual void Present(Transform parent = null)
    {
        gameObject.SetActive(true);
        transform.SetParent(parent);
        Transform.localPosition = Vector3.zero;
    }

    public void Present(Transform parent, Vector3 localPos, float scale)
    {
        Present(parent);
        transform.localScale = new Vector3(scale, scale, scale);
        transform.localPosition = localPos;
    }

    public Tween OnShow(float duration = 0.2f, float targetScale = 1f)
    {
        gameObject.SetActive(true);
        transform.localScale = new Vector3(.02f, .02f, .02f);
        var target = new Vector3(targetScale, targetScale, targetScale);
        return transform.DOScale(target, duration).SetEase(Ease.OutBack);
    }

    public Tween OnHide(float duration = 0.2f)
    {
        return transform.DOScale(new Vector3(.02f, .02f, .02f), duration).
            SetEase(Ease.InBack).OnComplete(Release);
    }

}

public abstract class PowerUpPresenter<T> : MonoPresenter<T> where T : IGift
{
    [SerializeField] protected Text infoText;
    [SerializeField] protected Text infoRewardDisplayText;
    public override void SetUpInfo(T reward)
    {
        infoRewardDisplayText.text = $"X{reward.GiftInfo}";
        infoText.text = $"X{reward.GiftInfo}";
    }
}