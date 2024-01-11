using JackieSoft;

public class EndlessGiftElement : EndlessGiftElementBase
{
    
}

public class EndlessGiftElementData : Cell.Data<EndlessGiftElement>
{
    public int id;
    public EndlessRewardPackage endlessRewardPackage;
    public IEndlessRewardStrategy endlessRewardStrategy;
    protected override void SetUp(EndlessGiftElement cellView)
    {
        cellView.Init(endlessRewardPackage, endlessRewardStrategy, id);
    }
}
