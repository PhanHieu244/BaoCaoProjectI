using System;
using CodeStage.AntiCheat.Storage;
using ColorRings.Runtime.GameMode.AdvancedBoard;

public abstract class WeekTimeTagNewStrategy : ITagNewStrategy
{
    protected abstract string GetKey(string id);
    public abstract TagNewType TagNewType { get; }

    public void Checked(string id)
    {
        ObscuredPrefs.Set(GetKey(id), WeekEventScheduler.GetWeekSinceStart());
    }

    public bool HasNew(string id)
    {
        var recent = ObscuredPrefs.Get(GetKey(id), -1);
        var now = WeekEventScheduler.GetWeekSinceStart();
        return recent != now;
    }
}

[Serializable]
public class ModeTagNewStrategy : WeekTimeTagNewStrategy
{
    private const string TagKey = "ModeTagNew";

    public override TagNewType TagNewType => TagNewType.ModeWeek;
    protected override string GetKey(string id) => TagKey + id;
    
}