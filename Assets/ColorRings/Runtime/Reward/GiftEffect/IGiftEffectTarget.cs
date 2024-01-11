using UnityEngine;

public interface IGiftEffectTarget
{ 
    Vector3 TargetPos { get; }
    void InvokeOnComplete();
}