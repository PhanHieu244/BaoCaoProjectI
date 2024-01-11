using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public abstract class IapPack {
    
    public virtual GameObject Element {
        get => null;
    }

    public virtual bool Available {
        get => true;
    }

    public abstract void Initialization(Transform parent);
}

[Serializable]
public abstract class IapPack<T> : IapPack where T : MonoBehaviour {
    [SerializeField] private T prefab;
    protected T element;

    public override GameObject Element {
        get => element == null ? null : element.gameObject;
    }

    public override void Initialization(Transform parent) {
        element = Object.Instantiate(prefab, parent);
    }
}