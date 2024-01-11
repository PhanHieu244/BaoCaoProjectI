using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class RingSpawner : MonoBehaviour
{
    [ReadOnly] public Level level;
    [SerializeField] private float[] ratio;
    [SerializeField] private RingHolder holder;
    [SerializeField] private SpriteRenderer skinShape;

    private int _ringAmountToIncreaseLevel = GameConst.TimesSpawnEndless;
    private static int COLOR_MAX = Enum.GetValues(typeof(Color)).Length;
    private const int COLOR_MIN = 1;
    private static int SIZE_MAX = Board.Depth;
    private const int SIZE_MIN = 0;

    public event Action<Color[]> OnForecast;
    private float[] _total;
    private List<(float ratio, Color[] value)> _pattern;
    private Queue<Color[]> _ringsQueue;
    private Color[] _ringInHolder;
    private int _timeSpawnEndless;

    private void Awake()
    {
        _total = new float[ratio.Length];
        var sum = 0f;
        for (var i = 0; i < ratio.Length; i++)
        {
            sum += ratio[i];
            _total[i] = sum;
        }
        _timeSpawnEndless = 0;
    }

    private void Start()
    {
        var sprite = GameManager.Instance.SkinShape;
        if (sprite is not null)
        {
            skinShape.gameObject.SetActive(true);
            skinShape.sprite = sprite;
            return;
        }
        skinShape.gameObject.SetActive(false);
    }

    public void Spawn(int[][] availablePattern)
    {
        int[] pattern;
        if (availablePattern == null)
            pattern = new[] { SIZE_MAX - 1 };
        else if (availablePattern.Length == 0)
            pattern = new[] { Random.Range(SIZE_MIN, SIZE_MAX) };
        else
            pattern = availablePattern[Random.Range(0, availablePattern.Length)];
        var totalRing = GetTotalRing(pattern.Length);
        for (var i = 0; i < totalRing; i++)
        {
            var color = (Color)Random.Range(COLOR_MIN, COLOR_MAX);
            var r = Random.Range(0, pattern.Length);
            var size = pattern[r];
            var ringEntity = PoolManager.Instance[((RingSize)size).ToString()].Get<RingEntity>(holder.transform);
            ringEntity.Order = holder.orderFront;
            holder[size] = ringEntity;
            ringEntity.Transform.localPosition = Vector3.zero;
            ringEntity.Transform.localScale = Vector3.one;
            ringEntity.transform.rotation = quaternion.identity;
            ringEntity.Color = color;
            pattern = pattern.Where(p => p != size).ToArray();
        }

        holder.transform.localScale = new Vector3(.02f, .02f, .02f);
        holder.transform.DOScale(new Vector3(1f, 1f, 1f), .2f).SetEase(Ease.OutBack);
    }

    private int GetTotalRing(int maxIndex)
    {
        var x = Random.Range(0, _total[maxIndex - 1]);
        for (var i = 0; i < maxIndex; i++)
        {
            if (x < _total[i]) return i + 1;
        }

        return 1;
    }

    public void SpawnNew()
    {
        if (level.tutorialMod != null && level.tutorialMod.IsSpawn())
        {
            var pt = level.tutorialMod.GetPattern();
            var indexColor = 0;
            while (pt.colors[indexColor] == Color.NONE)
            {
                indexColor++;
            }
            var colors = pt.colors[indexColor];
            var count = -100;
            var sizeStep = level.tutorialMod.SizeStep();
            if (count < sizeStep)
            {
                count++;
                var size = (RingSize)(level.tutorialMod.GetSizeCoord() >= 0 ? level.tutorialMod.GetSizeCoord() : 2);
                var ringEntity = PoolManager.Instance[size.ToString()].Get<RingEntity>(holder.transform);
                ringEntity.Order = holder.orderFront;
                ringEntity.Layer = "Dark";
                holder[size] = ringEntity;
                ringEntity.Transform.localPosition = Vector3.zero;
                ringEntity.Transform.localScale = Vector3.one;
                ringEntity.transform.rotation = quaternion.identity;
                ringEntity.Color = colors;
            }
            holder.transform.localScale = new Vector3(.02f, .02f, .02f);
            holder.transform.DOScale(new Vector3(1f, 1f, 1f), .2f).SetEase(Ease.OutBack);
            return;
        }

        InitNewRing();
        SpawnRingEntity();
    }

    private void SpawnRingEntity()
    {
        _ringInHolder = _ringsQueue.Dequeue();
        for (var i = 0; i < _ringInHolder.Length; i++)
        {
            if (_ringInHolder[i] is Color.NONE) continue;
            var size = (RingSize)i;
            var ringEntity = PoolManager.Instance[size.ToString()].Get<RingEntity>(holder.transform);
            ringEntity.Order = holder.orderFront;
            holder[size] = ringEntity;
            ringEntity.Transform.localPosition = Vector3.zero;
            ringEntity.Transform.localScale = Vector3.one;
            ringEntity.transform.rotation = quaternion.identity;
            ringEntity.Color = _ringInHolder[i];
            holder[(RingSize)i] = ringEntity;
        }
        //pushLayer For Tutorial Mode
        holder.transform.localScale = new Vector3(.02f, .02f, .02f);
        holder.transform.DOScale(new Vector3(1f, 1f, 1f), .2f).SetEase(Ease.OutBack);
        
        OnForecast?.Invoke(_ringsQueue.Peek());
    }

    private void InitNewRing()
    {
        var rate = Random.Range(0, _pattern[^1].ratio);
        var index = 0;
        var startRatio = 0f;
        while (index < _pattern.Count)
        {
            if (startRatio <= rate && rate < _pattern[index].ratio)
            {
                break;
            }
            startRatio = _pattern[index].ratio;
            index++;
        }

        index = Mathf.Min(index, _pattern.Count - 1);

        var patternValue = _pattern[index].value;
        _ringsQueue.Enqueue(patternValue);
    }

    private void InitNewRingBySize()
    {
        var pattern = GameManager.Instance.Board.GetAvailablePattern(level.sizePatterns, _ringInHolder);
        var colors = new Color[SIZE_MAX];
        foreach (var size in pattern.sizes)
        {
            colors[(int)size] = level.colorPatterns[Random.Range(0, level.colorPatterns.Count)].color;
        }
        _ringsQueue.Enqueue(colors);
    }

    public void SpawnEndless()
    {
        _timeSpawnEndless++;
        InitNewRingBySize();
        SpawnRingEntity();
        if (_timeSpawnEndless > 0 && _timeSpawnEndless % _ringAmountToIncreaseLevel == 0)
        {
            DevLog.Log("ringAmount", "increase");
            level.IncreaseColor();
            Set(level);
        }
    }

    public IEnumerator IESpawnBySizePattern(SizePattern pattern)
    {
        yield return holder.transform.DOScale(new Vector3(0f, 0f, 0f), .2f).SetEase(Ease.InBack).WaitForCompletion();
        SpawnBySizePattern(pattern);
        yield return holder.transform.DOScale(new Vector3(1f, 1f, 1f), .2f).SetEase(Ease.OutBack).WaitForCompletion();
    }

    private void SpawnBySizePattern(SizePattern pattern)
    {
        for (var i = 0; i < Board.Depth; i++)
        {
            if (holder[i] is null) continue;
            holder[i].ReleaseNoAnim();
            holder[i] = null;
        }
        
        for (var i = 0; i < pattern.sizes.Length; i++)
        {
            var color = level.colorPatterns[Random.Range(0, level.colorPatterns.Count)].color;
            var ringEntity = PoolManager.Instance[pattern.sizes[i].ToString()].Get<RingEntity>(holder.transform);

            var transform1 = ringEntity.Transform;
            transform1.localScale = Vector3.one;
            transform1.localPosition = Vector3.zero;

            ringEntity.Order = holder.orderFront;
            ringEntity.Color = color;

            holder[pattern.sizes[i]] = ringEntity;
        }
    }

    public void Set(Level level)
    {
        if (level.levelUpStrategy is not null)
        {
            if(_pattern is null) level.ResetColor(level.levelUpStrategy.ColorAmountStart);
            _ringAmountToIncreaseLevel = level.levelUpStrategy.GetAmountRingsToLevelUp(level.colorPatterns.Count);
            DevLog.Log("ring increase", _ringAmountToIncreaseLevel);
            DevLog.Log("ring start", level.colorPatterns.Count);
        }
        this.level = level;
        _pattern = new List<(float ratio, Color[] value)>(level.patterns.Length)
        {
            (level.patterns[0].ratio, level.patterns[0].value)
        };
        for (var i = 1; i < level.patterns.Length; i++)
        {
            _pattern.Add((_pattern[i - 1].ratio + level.patterns[i].ratio, level.patterns[i].value));
        }
        
        if (_ringsQueue is not null) return;
        _ringsQueue = new Queue<Color[]>();
        InitNewRing();
        _ringInHolder = _ringsQueue.Peek();
    }

    public void OnActiveForecaster()
    {
        if (_ringsQueue is null || _ringsQueue.Count < 1) return;
        OnForecast?.Invoke(_ringsQueue.Peek());
    }

    public Vector2 GetRingHolderPosition()
    {
        return holder.transform.position;
    }

    public RingHolder GetRingHolder()
    {
        return holder;
    }

}