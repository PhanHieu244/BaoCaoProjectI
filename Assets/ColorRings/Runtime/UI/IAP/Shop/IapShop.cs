using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NaughtyAttributes;
using Puzzle.UI;
using UnityEngine;

public class IapShop : MonoBehaviour, IPopUpContent {
    [SerializeReference, SubclassSelector] private List<IapPack> packs = new();
    [SerializeField] private Transform grid;
    
    private void OnDisable()
    {
        IAPCacheData.OnBuyIap -= Refresh;
    }

    private void OnEnable() {
        IAPCacheData.OnBuyIap += Refresh;
        Refresh();
    }

    private void Refresh()
    {
        foreach (var pack in packs) {
            if (pack.Available) {
                if (pack.Element == null) {
                    pack.Initialization(grid);
                }

                pack.Element.SetActive(true);
            }
            else {
                if (pack.Element != null) {
                    pack.Element.SetActive(false);
                }
            }
        }
    }

    [Button]
    public void Log() {
        Debug.Log(LoadCatalog().Aggregate((s, s1) => s1 + "\n" + s));
    }

    public static string[] LoadCatalog() {
        var type = typeof(IAPKey);
        var properties = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        return properties.Select(info => info.GetValue(null).ToString()).Append(null).ToArray();
    }
}