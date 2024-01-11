using System;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "EndlessRewardData", menuName = "Scriptable Objects/EndlessReward", order = 0)]
public class EndlessRewardDataSO : ScriptableObject
{
    [field: SerializeField] public EndlessRewardPackage[] EndlessRewardPackages { get; private set; }

    [Button()]
    private void SortByPoint()
    {
        EndlessRewardPackages = EndlessRewardPackages?.OrderBy(package => package.Point).ToArray();
    }
    
}

[Serializable]
public class EndlessRewardPackage
{
    [field: SerializeField] public int Point { get; private set; }
    [field: SerializeField] public GiftRewardPackage RewardPackage { get; private set; }
}