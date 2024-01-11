using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

public class RewardPool<TReward, TPresenter, TAtt, T> : MonoSingleton<T> where TAtt : RewardAttribute where TPresenter : class where T : MonoSingleton<T>
{
    public List<MonoBehaviour> RewardPresenters;
    public Dictionary<Type, ObjectPool<TPresenter>> Pools;
    
    public void Initialization() {
        Pools = new Dictionary<Type, ObjectPool<TPresenter>>();
        var cachePrefabs = new Dictionary<Type, GameObject>();
        foreach (var presenter in RewardPresenters) {
#if UNITY_EDITOR
            if (cachePrefabs.ContainsKey(presenter.GetType())) throw new DuplicateNameException($"{presenter.GetType()} is exist");
#endif
            cachePrefabs.Add(presenter.GetType(), presenter.gameObject);
        }

        var rewardTypeInterface = typeof(TReward);
        var rewardTypes = rewardTypeInterface.Assembly
            .GetTypes().Where(type => !type.IsAbstract && !type.IsInterface && rewardTypeInterface.IsAssignableFrom(type));

        foreach (var rewardType in rewardTypes) {
            var att = rewardType.GetCustomAttribute<TAtt>();
#if UNITY_EDITOR
            if (att is null) throw new ArgumentNullException($"{rewardType} has null attribute");
            if (Pools.ContainsKey(att.Target)) throw new DuplicateNameException($"{att.Target} is exist");
            if(!cachePrefabs.ContainsKey(att.Target)) DevLog.Log(att.Target, "dont have presenter in pool");
#endif
            if(!cachePrefabs.ContainsKey(att.Target)) continue;
            var prefab = cachePrefabs[att.Target];
            Pools.Add(rewardType, new ObjectPool<TPresenter>(
                () => Instantiate(prefab, transform).GetComponent<TPresenter>())
            );
        }
    }

    public ObjectPool<TPresenter> this[Type type] => Pools[type]; 

    public TPresenter Get<K>() where K : TReward {
        return Get(typeof(K));
    }
    public TPresenter Get(Type type) => Pools[type].Get();

    public void Release<K>(TPresenter presenter) where K : TReward => Pools[typeof(K)].Release(presenter);
}