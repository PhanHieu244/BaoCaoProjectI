using DG.Tweening;
using System;
using UnityEngine;

public class RingEntity : Entity 
{
    public static readonly int ColorProperty = Shader.PropertyToID("_Color");
    private SpriteRenderer _renderer;
    private Color _color;
    
    [SerializeField] private BrokenRing brokenRing;
    private BrokenRing _brokenRing;

    public override Color Color
    {
        get => _color;
        set
        {
            _renderer.sprite = GameManager.Instance.GetSkin(value, RingSize);
            _color = value;
        }
    }

    public override int Order
    {
        set => _renderer.sortingOrder = value;
    }

    public override string Layer
    {
        set => _renderer.sortingLayerName = value;
    }

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }


    public override void Release()
    {
        transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _color = Color.NONE;
            PoolManager.Instance[RingSize.ToString()].Release(this);
        }).Play();
    }

    public override void ReleaseNoAnim()
    {
        _color = Color.NONE;
        PoolManager.Instance[RingSize.ToString()].Release(this);
    }

    public void ReleaseBroken()
    {
        if (_brokenRing == null) _brokenRing = Instantiate(brokenRing, transform.parent);

        _brokenRing.transform.position = transform.position;

        _brokenRing.PlayEffect(_color);
        
        ReleaseNoAnim();
    }
    
}