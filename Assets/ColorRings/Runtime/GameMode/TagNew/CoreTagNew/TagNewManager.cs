using System;
using System.Collections.Generic;
using UnityEngine;

public class TagNewManager : MonoBehaviour
{
    [SubclassSelector, SerializeReference] private ITagNewStrategy[] _tagNewStrategies;
    private Dictionary<TagNewType, ITagNewStrategy> _tagNewDict;
    public Action onRefresh;
    public static TagNewManager Instance;

    private void Awake()
    {
        Instance = this;
        _tagNewDict = new Dictionary<TagNewType, ITagNewStrategy>();
        foreach (var tagNewStrategy in _tagNewStrategies)
        {
            _tagNewDict[tagNewStrategy.TagNewType] = tagNewStrategy;
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void Checked(TagNewType type, string id)
    {
        if (!_tagNewDict.ContainsKey(type)) return;
        _tagNewDict[type].Checked(id);
    }
    
    public bool HasNew(TagNewType type, string id) => _tagNewDict.ContainsKey(type) && _tagNewDict[type].HasNew(id) ;
}