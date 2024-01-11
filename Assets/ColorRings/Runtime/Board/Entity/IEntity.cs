using System;
using UnityEngine;

public interface IEntity
{ 
    Color Color { get;  }
    RingSize RingSize { get; }
    Transform Transform { get; }
    int Order { set; }
    string Layer { set; }
    void Release();
    void ReleaseNoAnim();
    
    
}

public abstract class Entity : MonoBehaviour, IEntity
{
    [field: SerializeField] public RingSize RingSize { get; protected set; }
    public abstract Color Color { get; set; }
    public Transform Transform => transform;
    public abstract int Order { set; }
    public abstract string Layer { set; }
    public abstract void Release();
    public abstract void ReleaseNoAnim();
    
}


