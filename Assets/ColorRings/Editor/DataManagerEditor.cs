using CodeStage.AntiCheat.Common;
using ColorRings.Runtime.GameData;
using UnityEditor;

namespace ColorRings.Editor
{
    public class DataManagerEditor : EditorWindow
    {
        [MenuItem("Tools/DataManager/DeletePlayerData")]
        public static void DeleteData()
        {
            UnityApiResultsHolder.InitForAsyncUsage(true);
            DataHandle.InitData();
            GameDataManager.DeleteAll();
        }
        
        [MenuItem("Tools/DataManager/HackCoin")]
        public static void HackCoin()
        {
            GameDataManager.AddCoins(1000);
        }
        
        [MenuItem("Tools/DataManager/UnLockAllSkin")]
        public static void UnlockSkin()
        {
            var shopData = GameDataManager.SkinShopData;
            foreach (var id in shopData.RewardSkinsID)
            {
                GameDataManager.UnlockSkinByID(id);
            }
            foreach (var id in shopData.ShopOnlySkinsID)
            {
                GameDataManager.UnlockSkinByID(id);
            }
        }
        
        [MenuItem("Tools/DataManager/UnLockEventSkin")]
        public static void UnlockEventSkin()
        {
            var shopData = GameDataManager.SkinShopData;
            foreach (var id in shopData.LockSkinsID)
            {
                GameDataManager.UnlockSkinByID(id);
            }
        }
        
        [MenuItem("Tools/DataManager/Pass10Level")]
        public static void PassLevel()
        {
            GameDataManager.MaxLevelUnlock += 10;
        }
    }
}