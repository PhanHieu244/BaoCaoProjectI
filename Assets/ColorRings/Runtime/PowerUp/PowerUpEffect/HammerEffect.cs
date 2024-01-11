using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerEffect : MonoBehaviour
{
    public void Release()
    {
        PoolManager.Instance["HammerEffect"].Release(this);
    }
}
