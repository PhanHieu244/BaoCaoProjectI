using System;
using Jackie.Soft;
using UnityEngine;

public abstract class Challenge<T> : MonoBehaviour, Message.ICallback where T : Challenge<T>, IChallenge
{

    protected virtual void OnEnable()
    {
        Message.Use<Type>().With(this).Sub(typeof(T));
    }
    
    protected virtual void OnDisable()
    {
        Message.Use<Type>().With(this).UnSub(typeof(T));
    }
}

public interface IChallenge
{
    bool IsComplete();
    void SetComplete();
}