using System;

[AttributeUsage(AttributeTargets.Class)]
public class RewardAttribute : Attribute
{
    public readonly Type Target;

    protected RewardAttribute(Type target) {
        Target = target;
    } 
}


public class PresenterAttribute : RewardAttribute
{
    public PresenterAttribute(Type target) : base(target)
    {
    }
}

public class TreasurePresenterAttribute : RewardAttribute
{
    public TreasurePresenterAttribute(Type target) : base(target)
    {
    }
}

public class EffectorAttribute : RewardAttribute
{
    public EffectorAttribute(Type target) : base(target)
    {
    }
}