namespace ColorRings.Runtime.GameData
{
    public class EndlessClassicData : IData
    {
        public int maxPointAllTime;
        public int highScore;
        public int currentRewardHighScore;
        public int[] receivedReward;

        public EndlessClassicData()
        {
            receivedReward = new int[20];
        }
    }
}