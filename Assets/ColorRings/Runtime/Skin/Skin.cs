using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Scriptable Objects/Skin", fileName = "Skin 0")]
public class Skin : ScriptableObject
{
    [SerializeField, ReadOnly] private Sprite[] skinImage;
    [field: SerializeField] public Sprite SkinShape { get; private set; }
    [field: SerializeField] public bool ShowFullSkin { get; private set; }
    private readonly int _colorAmount = (int) Color.COUNT;
    private readonly int _sizeAmount = Enum.GetNames(typeof(RingSize)).Length;

    public Sprite this[Color color, RingSize size] => skinImage[(int)(color - 1) * _sizeAmount + (int)size];
    public Sprite this[int color, int size] => skinImage[(color - 1) * _sizeAmount + size];

    
    public void SetUpSkin(Image[] ringImages, Color color = Color.NONE)
    {
        if (color == Color.NONE)
        {
            color = (Color) Random.Range(1,(int) Color.COUNT);
        }
        ringImages[(int)RingSize.SMALL_RING].sprite = this[color ,RingSize.SMALL_RING];
        ringImages[(int)RingSize.MEDIUM_RING].sprite = this[color ,RingSize.MEDIUM_RING];  
        ringImages[(int)RingSize.BIG_RING].sprite = this[color ,RingSize.BIG_RING]; 
    }

#if UNITY_EDITOR
    
    [field: Space][field: Header("Editor Setting")]
    [field: SerializeField] public string NameToLoad { get; private set; }
    [SerializeField, ReadOnly] private List<SkinData> skinListEditor;

    [Obsolete("EditorOnly")]
    public void InitData(string nameToLoad)
    {
        NameToLoad = nameToLoad;
        UpdateData();
        Save();
    }
    
    
    [Button("UpdateData")]
    private void UpdateData()
    {
        skinListEditor = new List<SkinData>();
        var tempList = new List<SkinData>();
        for (var sizeID = 0; sizeID < _sizeAmount; sizeID++)
        {
            var size = (RingSize)sizeID;
            for (var colorID = (int) Color.NONE + 1; colorID < _colorAmount; colorID++)
            {
                var color = (Color)colorID;
                var skinImage = GetSprite(color, size);
                tempList.Add(new SkinData
                {
                    color = color,
                    size = size,
                    skinImage = skinImage
                });
            }
        }


        SkinShape = GetShape();
        skinListEditor = tempList;
        InitSkinData();
    }

    [Button]
    private void Save()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssetIfDirty(this);
        AssetDatabase.Refresh();
    }

    private Sprite GetShape()
    {
        string path;
        string searchString = "size";
        if (NameToLoad.Contains(searchString))
        {
            int index = NameToLoad.IndexOf(searchString, StringComparison.Ordinal);
            var newLoad = NameToLoad.Substring(0, index + searchString.Length);
            path = $"Assets/ColorRings/Sprites/WoodTheme/Rings_skin/{newLoad}/shape.png";
        }
        else
        {
            path = $"Assets/ColorRings/Sprites/WoodTheme/Rings_skin/{NameToLoad}/shape.png";
        }
        
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if(sprite is null) DevLog.Log(path);
        return sprite;
    }

    private void InitSkinData()
    {
        skinImage = new Sprite[(_colorAmount - 1) * _sizeAmount];
        for (var colorID = (int) Color.NONE + 1; colorID < _colorAmount; colorID++)
        {
            var color = (Color)colorID;
            for (var sizeID = 0; sizeID < _sizeAmount; sizeID++)
            {
                var size = (RingSize)sizeID;
                skinImage[(colorID - 1) * _sizeAmount + sizeID] = GetSkin(color, size);
            }
        }
    }

    private Sprite GetSprite(Color color, RingSize size)
    {
        string path;
        if (NameToLoad.Equals("WoodTheme"))
        {
            path = $"Assets/ColorRings/Sprites/{NameToLoad}/board/COLOR/{GetStringColor(color)}/{GetStringSize(size)}.png";
        }
        else
        {
            path = $"Assets/ColorRings/Sprites/WoodTheme/Rings_skin/{NameToLoad}/{GetStringSize(size)}/{GetStringColor(color)}.png";
        }

        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if(sprite is null) DevLog.Log(path);
        return sprite;
    }
 
    private string GetStringColor(Color color)
    {
        return color switch
        {
            Color.RED => "RED",
            Color.GREEN => "GREEN",
            Color.ORANGE => "ORANGE",
            Color.PINK => "PINK",
            Color.YELLOW => "YELLOW",
            Color.CYAN => "CYAN",
            Color.WHITE => "WHITE",
            Color.BLUE => "BLUE",
            Color.PURPLE => "PURPLE",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private char GetStringSize(RingSize size)
    {
        switch (size)
        {
            case RingSize.SMALL_RING:
                return 'S';
            case RingSize.MEDIUM_RING:
                return 'M';
            case RingSize.BIG_RING:
                return 'L';
            default:
                Debug.Log("Color Not Find");
                return 'X';
        }
    }

    private Sprite GetSkin(Color color, RingSize size)
    {
        var skin = skinListEditor.FirstOrDefault(skin => skin.color == color && skin.size == size);
        if(skin is null) Debug.Log($"sprite of {size} {color} is null");
        return skin?.skinImage;
    }
#endif
    
}

[Serializable]
public class SkinData
{
    public RingSize size;
    public Color color;
    public Sprite skinImage;

}
