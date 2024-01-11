using System;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "PreciousTreasureData", menuName = "Scriptable Objects/PreciousTreasure", order = 0)]
public class PreciousTreasureSO : ScriptableObject
{
    [SerializeField] private TreasureChest[] treasureChests;

    [Button()]
    private void SortItemByRingAmount()
    {
        Array.Sort(treasureChests, (item1, item2) =>
        {
            int ringsAmount1 = item1.BreakRingsMission?.RingsAmount ?? 0;
            int ringsAmount2 = item2.BreakRingsMission?.RingsAmount ?? 0;

            return ringsAmount1.CompareTo(ringsAmount2);
        });
    }

    public TreasureChest this[int treasureID]
    {
        get
        {
            if (treasureID >= treasureChests.Length) return null;
            return treasureChests[treasureID];
        }
    }
}

[Serializable]
public class TreasureChest
{
    [field: SerializeField] public BreakRingsMission BreakRingsMission { get; private set; }
    [field: SubclassSelector, SerializeReference] public IGift Gift { get; private set; }
}