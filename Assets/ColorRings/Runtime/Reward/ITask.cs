public interface ITask
{
    bool IsTaskComplete();
    void RewardTask();
}

public abstract class MissionTask : ITask
{
    public abstract bool IsTaskComplete();

    public abstract void RewardTask();
}