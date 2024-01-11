using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalEffect : MonoBehaviour
{
    public void Release()
    {
        PoolManager.Instance["LeftHorizontalEffect"].Release(this);
    }
}
