using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
    private readonly Dictionary<string, Pool> dictionary = new Dictionary<string, Pool>();

    public bool Contains(string key) => dictionary.ContainsKey(key);

    public Pool this[string key] => dictionary.ContainsKey(key) ? dictionary[key] : null;

    public void Create(string key, Component objectPooled, int initCapacity = 10)
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(key)) throw new InvalidDataException($"{nameof(key)} is null or empty");
        if (dictionary.ContainsKey(key)) throw new DuplicateNameException($"{key} is exist");
#endif
        var pool = gameObject.AddComponent<Pool>();
        pool.key = key;
        pool.initCapacity = initCapacity;
        pool.objectPooled = objectPooled;
        pool.Initialize();
        dictionary.Add(key, pool);
    }

    public void DestroyImmediate(string key)
    {
        if (!dictionary.ContainsKey(key)) return;
        DestroyImmediate(dictionary[key]);
        dictionary.Remove(key);
    }

    protected override void Awake()
    {
        base.Awake();
        var pools = GetComponentsInChildren<Pool>();
        foreach (var pool in pools)
        {        
            if (string.IsNullOrEmpty(pool.key)) throw new InvalidDataException($"{nameof(Pool.key)} is null or empty");
            if (dictionary.ContainsKey(pool.key)) throw new DuplicateNameException($"{pool.key} is exist");
            dictionary.Add(pool.key, pool);
        }
    }
}