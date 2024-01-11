using CodeStage.AntiCheat.Storage;
using UnityEngine;

namespace ColorRings.Runtime.GameData
{
    public interface IData
    {
    }
    public static class DataHandle
    {
        /// <summary>
        /// Need to use UnityApiResultsHolder.InitForAsyncUsage in mainThread before Init
        /// </summary>
        public static void InitData()
        {
            DevLog.Log("InitData");
            ObscuredFilePrefs.Init();
        }
        
        /// <summary>
        /// Returns the value corresponding to <c>key</c> in the preference file if it exists.
        /// If it doesn't exist, it will return <c>defaultValue</c>.
        /// </summary>
        public static T GetData<T>(string key) where T : IData, new()
        {
            var json = ObscuredFilePrefs.Get(key, "");
            DevLog.Log("load: " + json);
            return json.Equals("") ? new T() : JsonUtility.FromJson<T>(json);
        }

        /// <summary>
        /// save new data 
        /// </summary>
        public static void Save<T>(string key, T data) where T : IData
        {
            var json = JsonUtility.ToJson(data);
            DevLog.Log("save: " + json);
            ObscuredFilePrefs.Set(key, json);
        }

        /// <summary>
        /// delete data in this key
        /// </summary>
        public static void ClearData(string key)
        {
            ObscuredFilePrefs.DeleteKey(key);
        }
    }
}