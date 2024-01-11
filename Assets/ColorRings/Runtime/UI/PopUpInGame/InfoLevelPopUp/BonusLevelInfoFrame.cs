public class BonusLevelInfoFrame : InfoLevelFrame
{
    private int _currentLevel = -1;
    public override void InitFrameData(int currentLevel = -1)
    {
        _currentLevel = currentLevel;
        playBut.onClick.RemoveAllListeners();
        playBut.onClick.AddListener((() =>
        {
            AudioManager.Instance.PlaySound(EventSound.Click);
            GameLoader.Instance.Load(_currentLevel);
        }));
    }
}