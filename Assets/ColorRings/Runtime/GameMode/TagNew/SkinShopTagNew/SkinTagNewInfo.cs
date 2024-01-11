using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkinTagNewInfo : ITagNewInfo
{
    [SerializeField] public SkinItem skinItem;
    public TagNewType TagNewType => TagNewType.ModeSkin;
    public string Id => skinItem == null ? "0" : skinItem.GetSkinId().ToString();
}

[Serializable]
public class SkinTabBarTagNewInfo : ITagNewInfo
{
    public TagNewType TagNewType => TagNewType.ModeSkinTab;

    public string Id => "SkinTab";
}