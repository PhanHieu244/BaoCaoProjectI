#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
#endif

using System.Text.RegularExpressions;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/HashSkinData", fileName = "HashSkinData")]
public class HashSkinData : ScriptableObject
{
    [field: SerializeField, ReadOnly] public string[] ResourceSkinNames { get; private set; }
    [field: SerializeField, ReadOnly] public int[] UnAvailableInGame { get; private set; }
    public string this[int id] => ResourceSkinNames[id]; 

#if UNITY_EDITOR
    [field: SerializeField] public List<HashIdData> HashIds { get; private set; }

    private const string FolderPath = "Assets/Resources/Skin Selector/Skin";
    private bool[] _checkList;
    

    [Obsolete("EditorOnly")]
    public string GetNameToLoadByNameSkin(string skinName)
    {
        foreach (var hashIdData in HashIds.Where(hashIdData => hashIdData.resourceName.Equals(skinName)))
        {
            return hashIdData.nameSkin;
        }
        DevLog.LogError($"Dont have this skinName{skinName}");
        return null;
    }
    
    [Obsolete("EditorOnly")]
    public int HashIDByNameToLoad(string nameToLoad)
    {
        foreach (var hashIdData in HashIds.Where(hashIdData => hashIdData.nameSkin.Equals(nameToLoad)))
        {
            return hashIdData.id;
        }
        DevLog.LogError($"dont have this name NameToLoad:{nameToLoad}");
        return 0;
    }

    [Button]
    private void InitSkinDataID()
    {
        string folderPath = "Assets/ColorRings/Sprites/WoodTheme/Rings_skin/";
        string savePath = "Assets/Resources/Skin Selector/Skin/";
        if (HashIds is null) HashIds = new List<HashIdData>();
        if (Directory.Exists(folderPath))
        {
            string[] directories = Directory.GetDirectories(folderPath);
            var checkID = new Dictionary<int, bool>();
            foreach (string directory in directories)
            {
                var folderName = Path.GetFileName(directory);
                var skin = ScriptableObject.CreateInstance<Skin>();
                var skinPath = savePath + folderName + ".asset";
                
                string pattern = @"(\d+)\..*";
                System.Text.RegularExpressions.Match match = Regex.Match(folderName, pattern);
                if (match.Success)
                {

                    string numberString = match.Groups[1].Value;
                    
                    if (int.TryParse(numberString, out int result))
                    {
                        while (HashIds.Count <= result)
                        {
                            HashIds.Add(new HashIdData());
                        }

                        if (checkID.ContainsKey(result))
                        {
                            DevLog.Log("this key is exist:", result);
                            continue;
                        }

                        checkID[result] = true;
                        bool available = true;
                        bool isHasCache = false;
                        Skin cache = CreateInstance<Skin>();
                        if (HashIds[result].id == result)
                        {
                            isHasCache = true;
                            cache = Resources.Load<Skin>("Skin Selector/Skin/" + HashIds[result].resourceName);
                            AssetDatabase.RenameAsset(savePath + HashIds[result].resourceName + ".asset", folderName);
                            AssetDatabase.Refresh();
                            available = HashIds[result].availableInGame;
                            if (result == 0) available = true;
                        }

                        HashIds[result] = new HashIdData
                        {
                            id = result,
                            nameSkin = folderName,
                            resourceName = folderName,
                            availableInGame = available
                        };
                        if (isHasCache && cache is not null) skin = cache;
                        skin.InitData(folderName);
                        if (!Directory.Exists(skinPath) && !isHasCache)
                        {
                            AssetDatabase.CreateAsset(skin, skinPath);
                        }
                    }
                    else
                    {
                        DevLog.Log("folder skin name is error");
                    }
                }
                
            }

            var unavailableList = new List<int>();
            ResourceSkinNames = new string[HashIds.Count];
            for (int i = 0; i < HashIds.Count; i++)
            {
                if (!HashIds[i].availableInGame)
                {
                    HashIds[i].id = i;
                    unavailableList.Add(i);
                    continue;
                }
                if(HashIds[i].id == i) continue;
                HashIds[i].id = i;  
                HashIds[i].availableInGame = false;
                unavailableList.Add(i);
                DevLog.Log("Dont have this id:", i);
            }
            
            foreach (var hashId in HashIds)
            {
                ResourceSkinNames[hashId.id] = hashId.resourceName;
                if(hashId.availableInGame) continue;
            }



            UnAvailableInGame = unavailableList.ToArray();
        }
        else
        {
            DevLog.LogError("this Path is not exist");
        }
    }

    [Serializable]
    public class HashIdData
    { 
        public int id;
        public string nameSkin;
        public string resourceName;
        public bool availableInGame;
    }
#endif
}
