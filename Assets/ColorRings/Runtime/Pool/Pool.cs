using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Pool : MonoBehaviour
{
    public string key;
    public Component objectPooled;
    public int initCapacity = 10;
    private readonly Queue<Component> queue = new();

    private void Awake()
    {
        Initialize();
    }
    
    public void Initialize()
    {
        if (initCapacity == 0 || objectPooled == null) return;
        for (var i = 0; i < initCapacity; i++)
        {
            var obj = Instantiate(objectPooled, transform);
            obj.gameObject.SetActive(false);
            queue.Enqueue(obj);
        }
    }

    public T Get<T>() where T : Component
    {
#if UNITY_EDITOR
        if (typeof(T) != objectPooled.GetType()) throw new InvalidDataException($"{typeof(T)} is not {objectPooled.GetType()}");
#endif
        var obj = queue.Count > 0 ? queue.Dequeue() : Instantiate(objectPooled, transform);
        obj.transform.SetParent(null, false);
        obj.gameObject.SetActive(true);
        return obj as T;
    }

    public void Release<T>(T obj) where T : Component
    {
#if UNITY_EDITOR
        if (obj.GetType() != objectPooled.GetType()) throw new InvalidDataException($"{obj.GetType()} is not {objectPooled.GetType()}");
#endif
        obj.transform.SetParent(transform, false);
        obj.gameObject.SetActive(false);
        queue.Enqueue(obj);
    }
    
    public T Get<T>(Transform parent) where T : Component
    {
        var obj = queue.Count > 0 ? queue.Dequeue() : Instantiate(objectPooled, transform);
        obj.transform.SetParent(parent);
        obj.gameObject.SetActive(true);
        return obj as T;
    }
}