using System;
using DG.Tweening;
using UnityEngine;

public class Combo : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite x3, x4, x5, x6, good, wow, unbelievable;
    private Camera _cameraMain;
    private float _endValueScale = 1f;

    private void Awake()
    {
        _cameraMain = Camera.main;
    }

    public int Count
    {
        set
        {
            _renderer.sprite = value switch
            {
                3 => x3,
                4 => x4,
                5 => good,
                6 => wow,
                > 6 => unbelievable,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }
    }

    private void SetUpScale()
    {
        if (_cameraMain is null) return;
        _endValueScale = _cameraMain.orthographicSize / SettingBoard.ClassicCameraSize;
    }

    public Tween Animation(Vector3 position)
    {
        var timeIn = 0.2f;
        var timeOut = 0.2f;
        transform.position = position;
        transform.rotation = Quaternion.Euler(0, 0, -180);
        transform.localScale = Vector3.zero;
        SetUpScale();
        return DOTween.Sequence()
            .Append(
                DOTween.Sequence()
                    .Append(transform.DOScale(_endValueScale, timeIn))
                    .Join(transform.DORotate(Vector3.zero, timeIn, RotateMode.FastBeyond360))
                    .SetEase(Ease.OutBack)
            ).AppendInterval(0.3f).Append(
                DOTween.Sequence()
                    .Append(transform.DOScale(0, timeOut))
                    .Join(transform.DORotate(new Vector3(0, 0, 180), timeOut, RotateMode.FastBeyond360))
                    .SetEase(Ease.InBack)
            ).AppendCallback(() => PoolManager.Instance["Combo"].Release(this));
    }
}