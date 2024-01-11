using JackieSoft;
using UnityEngine;
using UnityEngine.UI;

namespace ColorRings.Runtime.UI.Navigator.HomeNavigation
{
    public class LevelActiveButtonScroll: LevelButtonScrollBase
    {
        [Header("Setup BG")]
        [SerializeField] private Image imageBg;
        [SerializeField] private Sprite normalLevelBg;
        [SerializeField] private Sprite bonusLevelBg;

        public override void Init(string level, LevelScrollType levelScrollType)
        {
            imageBg.sprite = levelScrollType == LevelScrollType.Bonus ? bonusLevelBg : normalLevelBg;
            base.Init(level, levelScrollType);
        }
    }
    
    public class LevelActiveButtonScrollData : Cell.Data<LevelActiveButtonScroll>
    {
        public string level;
        public LevelScrollType levelScrollType;

        protected override void SetUp(LevelActiveButtonScroll cellView)
        {
            cellView.Init(level, levelScrollType);
        }
    }

}