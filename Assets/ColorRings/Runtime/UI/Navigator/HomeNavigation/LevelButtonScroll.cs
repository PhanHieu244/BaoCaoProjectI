using JackieSoft;

public class LevelButtonScroll : LevelButtonScrollBase
{
    
}

public class LevelButtonScrollData : Cell.Data<LevelButtonScroll>
{
    public string level;
    public LevelScrollType levelScrollType;
    protected override void SetUp(LevelButtonScroll cellView)
    {
        cellView.Init(level, levelScrollType);
    }
}


