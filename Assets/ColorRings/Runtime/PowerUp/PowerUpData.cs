using CodeStage.AntiCheat.Storage;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/PowerUpData", fileName = "PowerUpData", order = 0)]
public class PowerUpData : ScriptableObject
{
    public int Count
    {
        get => ObscuredPrefs.Get(type.ToString(), 0);
        set => ObscuredPrefs.Set(type.ToString(), value);
    }
    
    public PowerUpType type;

    public override string ToString() => type.ToString();
}

public enum PowerUpType
{
    Tile,
    Swap,
    Row,
    Line,
}