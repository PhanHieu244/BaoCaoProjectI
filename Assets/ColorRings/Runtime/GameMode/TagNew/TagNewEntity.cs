using UnityEngine;

public class TagNewEntity : TagNew
{
    [SerializeField] protected GameObject tagEntity;
    [SerializeField] private bool nonAnim;
    protected bool isShowTag = true;
    protected virtual void OnEnable()
    {
        if (TagNewManager.Instance is null) return;
        Refresh();
        if (!tagEntity.activeSelf) return;
        if (nonAnim) return;
    }

    protected virtual void Start()
    {
        TagNewManager.Instance.onRefresh += Refresh;
        Refresh();
    }

    protected virtual void OnDestroy()
    {
        if (TagNewManager.Instance is null) return;
        TagNewManager.Instance.onRefresh -= Refresh;
    }

    protected virtual void Refresh()
    {
        isShowTag = true;
        tagEntity.SetActive(HasNew());
    }

    protected virtual void UnActiveTag()
    {
        isShowTag = false;
        tagEntity.SetActive(false);
    }
}