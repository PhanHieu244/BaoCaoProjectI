using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/InGamePowerUpConfig", fileName = "InGamePowerUpConfig", order = 0)]
public class InGamePowerUpConfig : ScriptableObject, IPowerUpConfig
{
    public Sprite sprite;
    public PowerUpType type;
}