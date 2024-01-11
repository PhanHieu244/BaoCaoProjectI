using System.Linq;
using UnityEngine;

public class TagNew : MonoBehaviour, ITagNew
{
    [SerializeReference, SubclassSelector] private ITagNewInfo[] _tagNewInfos;

    public void CheckedAll()
    {
        foreach (var tagNewInfo in _tagNewInfos)
        {
            TagNewManager.Instance.Checked(tagNewInfo.TagNewType, tagNewInfo.Id);
        }
    }

    public bool HasNew()
    {
        return _tagNewInfos.Any(tagNewInfo => TagNewManager.Instance.HasNew(tagNewInfo.TagNewType, tagNewInfo.Id));
    }
}

public interface ITagNew
{
    void CheckedAll();
    bool HasNew();
}
