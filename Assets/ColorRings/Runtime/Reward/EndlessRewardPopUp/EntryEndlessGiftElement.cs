using JackieSoft;

public class EntryEndlessGiftElement : EndlessGiftElementBase
{
        
}

public class EntryEndlessGiftElementData : Cell.Data<EntryEndlessGiftElement>
{
    public int id;
    public EndlessRewardPackage endlessRewardPackage;
    public IEndlessRewardStrategy endlessRewardStrategy;
    protected override void SetUp(EntryEndlessGiftElement cellView)
    {
        cellView.Init(endlessRewardPackage, endlessRewardStrategy, id);
    }

}