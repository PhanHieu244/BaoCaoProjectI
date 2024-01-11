using System;
using UnityEngine;

[Serializable]
public class ModeTagNewInfo : ITagNewInfo
{
    [SerializeField] private ModeTagNewType modeTagNewType;
    public TagNewType TagNewType => TagNewType.ModeWeek;
    public string Id => modeTagNewType.ToString();
}

public enum ModeTagNewType
{
    EndlessAdvanced
}