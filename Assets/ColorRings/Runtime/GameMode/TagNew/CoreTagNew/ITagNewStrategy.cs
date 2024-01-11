using System;
using CodeStage.AntiCheat.Storage;
using ColorRings.Runtime.GameMode.AdvancedBoard;
using UnityEngine;

public interface ITagNewStrategy
{
    TagNewType TagNewType { get; }
    void Checked(string id);
    bool HasNew(string id);
}

public enum TagNewType
{
    ModeWeek,
    ModeSkin,
    ModeSkinTab
}

