using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using ColorRings.Runtime.UI;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TrayWinningStreakContainer : MonoBehaviour
{
    [SerializeField] private Transform[] posTrails;
    [SerializeField] private Sprite tile, col, row, swap;
    [SerializeField] private WinningStreakItem item;
    [SerializeField] private Transform itemsContain;
    [SerializeField] private BlockUI blockUI;
    
    private static readonly int Disappear = Animator.StringToHash("disappear");
    private static readonly int Appear = Animator.StringToHash("appear");
    private List<ParticleSystem> _winningStreakTrails;
    private List<WinningStreakItem>  _itemsInTray;
    private Animator _animator;


    private void Awake()
    {
        blockUI.Block();
        _winningStreakTrails = new List<ParticleSystem>();
        _itemsInTray = new List<WinningStreakItem>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        
        ActiveTrails();
        _animator.SetBool(Appear, true);
        StartCoroutine(IEOpen());
    }

    private IEnumerator IEOpen()
    {
        yield return new WaitForSeconds(2f);
        foreach (var winningStreakItem in _itemsInTray)
        {
            //winningStreakItem.transform.SetParent(transform.parent);
            StartCoroutine(winningStreakItem.MoveToItem(transform.parent));
            yield return new WaitForSeconds(0.42f);
        }
        yield return new WaitForSeconds(1.3f);
        OnDisappear();
        yield return new WaitForSeconds(2f);
        blockUI.UnBlock();
        gameObject.SetActive(false);
    }

    public void Setup(PowerUpType[] powerUpTypes)
    {
        var itemCount = powerUpTypes.Length;
        var itemRect = item.GetComponent<RectTransform>().rect;
        var deltaX = itemCount switch
        {
            1 => new[] { 0f },
            2 => new[] { itemRect.height * 0.65f, -itemRect.height * 0.65f },
            3 => new[] { itemRect.height * 0.8f, -itemRect.height * 0.8f, 0 },
            4 => new[]
            {
                itemRect.height * 0.2f, -itemRect.height * 0.2f,
                itemRect.height * 0.65f, -itemRect.height * 0.65f
            },
            _ => throw new ArgumentOutOfRangeException()
        };

        for (var index = 0; index < itemCount; index++)
        {
            var powerUpType = powerUpTypes[index];
            var itemWS = Instantiate(item, itemsContain);
            itemWS.transform.localPosition += new Vector3(deltaX[index], 0, 0);
            itemWS.Setup(powerUpType, GetSpriteItem(powerUpType));
            _itemsInTray.Add(itemWS);
        }
    }
    
    private void ActiveTrails()
    {
        foreach (var posTrail in posTrails)
        {
            var trail = PoolManager.Instance["WinningStreakTrail"].Get<ParticleSystem>(posTrail);
            trail.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            //todo instantiate fx
            _winningStreakTrails.Add(trail);
            trail.Play();
        }
    }

    private void RemoveTrail()
    {
        foreach (var winningStreakTrail in _winningStreakTrails)
        {
            winningStreakTrail.Pause();
            PoolManager.Instance["WinningStreakTrail"].Release(winningStreakTrail);
        }
    }

    private void OnDisappear()
    {
        _animator.SetBool(Disappear, true);
    }
    
    private Sprite GetSpriteItem(PowerUpType powerUpType)=> powerUpType switch
    {
        PowerUpType.Tile => tile,
        PowerUpType.Row => row,
        PowerUpType.Line => col,
        PowerUpType.Swap => swap,
        _ => throw new ArgumentOutOfRangeException(nameof(powerUpType), powerUpType, null)
    };
        
}