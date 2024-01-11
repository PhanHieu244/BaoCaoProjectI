using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class horizontalLight : MonoBehaviour
{
    public void Release()
    {
        var sprite = GetComponent<SpriteRenderer>();
        var startValue = sprite.color.a;
        var newColor = sprite.color;
        var endColor = sprite.color;
        DOTween.To(() => startValue, c => 
        {
            newColor.a = c;
            sprite.color = newColor;
        }, 0f, 0.2f).OnComplete(()=>
        {
            PoolManager.Instance["horizontalLight"].Release(this);
            sprite.color = endColor;
            this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        });
    }
}
