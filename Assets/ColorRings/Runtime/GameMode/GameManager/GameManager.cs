using System;
using System.Collections;
using System.Collections.Generic;
using Jackie.Soft;
using NaughtyAttributes;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SubclassSelector, SerializeReference] protected IMatch matchStrategy = new NormalMatch();
    [SerializeField] protected BoardBaseEntity boardEntity;
    [SerializeField] private RingSpawner ringSpawner;
    [SerializeField] private Shader shader;
    [SerializeField] private Transform comboContainer;
    [SerializeField] private List<Wrapper<Color, Color32>> particleColors;
    [HideInInspector]public GameState gameState;
    public event Action<int> OnComboComplete; 
    public event Action OnWinGame; 
    public event Action OnLoseGame;
    public Action onOutGame;
    protected IBoard board;
    private Dictionary<Color, Material> _materialsParticle;
    private Level _level;
    private Skin _skin;

    public int LoseCount { get; private set; }
    public int RingsRemain { get; private set; }
    
    public IBoard Board
    {
        get => board;
        set => board = value;
    }

    public BoardBaseEntity BoardEntity
    {
        get => boardEntity;
        set => boardEntity = value;
    }

    public RingSpawner RingSpawner
    {
        get => ringSpawner;
    }

    public Level Level
    {
        get => _level;
    }

    protected override void Awake()
    {
        base.Awake();
        _materialsParticle = particleColors.ToDictionary(c => c, c32 =>
        {
            var material = new Material(shader);
            material.SetColor(RingEntity.ColorProperty, c32);
            return material;
        });
    }

    protected virtual void Start()
    {
        InGamePowerUpManager.Instance.OnRevival += DestroyTiles;
        InitBoard();
    }

    private void OnDisable()
    {
        InGamePowerUpManager.Instance.OnRevival -= DestroyTiles;
        OnWinGame = null;
        OnLoseGame = null;
    }

    public Sprite GetSkin(Color color, RingSize ringSize) => _skin[color, ringSize];
    public bool ShowFullSkin => _skin.ShowFullSkin;
    public Sprite SkinShape => _skin.SkinShape;
    public Material ParticleMaterial(Color color) => _materialsParticle[color];

    public virtual void SetUp(Level level, Skin skin)
    {
        _skin = skin;
        gameState = GameState.Playing;
        LoseCount = 0;
        ringSpawner.Set(level);
        StartCoroutine(IEStart(level));
        GameLog.Instance.OnStartGame();
        AdsManager.Instance.ResetTimesAdsBreak();
    }

    private IEnumerator IEStart(Level level)
    {
        this._level = level;
        
        // read data from board and init ring entity to board entity
        yield return new WaitForSeconds(0.5f);
        Message.Use<Type>().Event(typeof(IStartEvent)).Execute<IStartEvent>(e => e.OnGameStart()); 
        ringSpawner.SpawnNew();
        if (level.tutorialMod != null)
        {
            level.tutorialMod.InitializeBoard(this);
            level.tutorialMod.StartTutorial();
        }
    }

    protected virtual void InitBoard()
    { 
        boardEntity.InitBoardData(out board);
        matchStrategy.Initialize(board);
    }

    public IEnumerator IEInsert(RingHolder ringHolder)
    {
        RingHolder.IsLock = true;
        var coordinate = boardEntity.InputCoordinateAvailable(ringHolder.transform.position);

        if (coordinate == null)
        {
            yield return IEMoveTo(ringSpawner.transform.position, ringHolder.transform, 0.2f);
            goto end_insert;
        }

        var colors = (Color[])ringHolder;
        if (!board[coordinate.Value].IsAvailable(colors))
        {
            yield return IEMoveTo(ringSpawner.transform.position, ringHolder.transform, 0.2f);
            goto end_insert;
        }

        // update data to board
        var tile = board[coordinate.Value];
        for (var i = 0; i < Board.DEPTH; i++)
        {
            if (colors[i] != Color.NONE)
            {
                tile[i] = colors[i];
                boardEntity[coordinate.Value][i] = ringHolder[i];
            }
        }

        board[coordinate.Value] = tile;

        // play animation 
        yield return IEMoveTo(boardEntity[coordinate.Value].transform.position, ringHolder.transform, 0.2f);

        // check & apply match
        CheckAndApplyMatch(colors, coordinate.Value);

        // clear holder
        for (var i = 0; i < Board.DEPTH; i++)
        {
            if (ringHolder[i] == null) continue;
            ringHolder[i].Order = ringHolder.orderBack;
            ringHolder[i].Transform.parent = boardEntity.transform;
            ringHolder[i] = null;
        }

        ringHolder.transform.position = ringSpawner.transform.position;

        SpawnRing();

        var newColors = (Color[])ringHolder;
        for (var x = 0; x < board.Width; x++)
        {
            for (var y = 0; y < board.Height; y++)
            {
                if (!board.HasTile(x, y)) continue;
                if (board[x, y].IsAvailable(newColors)) goto end_insert;
            }
        }

        Message.Use<Type>().Event(typeof(ILoseEvent)).Execute<ILoseEvent>(e => e.OnGameLose());
        OnLose();
        end_insert:
        RingHolder.IsLock = false;
    }

    protected virtual void CheckAndApplyMatch(Color[] colors, Vector2Int coordinate)
    {
        var colorsMatch = new List<Color>();
        foreach (var color in colors)
        {
            if (color is Color.NONE or Color.COUNT || colorsMatch.Contains(color)) continue;
            colorsMatch.Add(color);
        }
        
        var comboCount = 0;
        
        foreach (var color in colorsMatch)
        {
            var matches = matchStrategy.Matches(coordinate, color, out var direction);
            if (matches.Count == 0) continue;
            
            foreach (var match in matches)
            {
                matchStrategy.Apply(match);
                boardEntity.Apply(match, color);
                Message.Use<Type>().Event(typeof(ColorChallenge)).Execute<ColorChallenge>(execution => execution.Check(match.Size.Length, color));
                comboCount += match.Size.Length;
            }

            boardEntity.BoardEffect.FlashEffect(matchStrategy, color, direction);
        }

        ActionCombo(comboCount);

        Message.Use<Type>().Event(typeof(ChallengeManager)).Execute<ChallengeManager>(c => c.Submit());

    }
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            AutoWin();
        }

        if (Input.GetKey(KeyCode.Q))
        {
            AutoLose();
        }
    }

    [Button()]
    private void AutoWin()
    {
        if(gameState == GameState.Win) return;
        for (int colorId = (int) Color.NONE + 1; colorId < (int) Color.COUNT; colorId++)
        {
            Message.Use<Type>().Event(typeof(ColorChallenge)).Execute<ColorChallenge>
                (execution => execution.Check(1000, (Color) colorId));
        }
        Message.Use<Type>().Event(typeof(ChallengeManager)).Execute<ChallengeManager>(c => c.Submit());
    }
    
    [Button()]
    private void AutoLose()
    {
        if(gameState == GameState.Lose) return;
        Message.Use<Type>().Event(typeof(ILoseEvent)).Execute<ILoseEvent>(e => e.OnGameLose());
        OnLose();
    }
#endif

    private void DestroyTiles(Vector2Int[] tileCoords)
    {
        gameState = GameState.Playing;
        PhoneManager.Instance.Viberate(100);
        var matches = new List<Match>();
        foreach (var tileCoord in tileCoords)
        {
            var tile = board[tileCoord];

            for (var i = 0; i < Board.DEPTH; i++)
            {
                var color = tile[i];
                if (color == Color.NONE) continue;
                tile[i] = Color.NONE;
                boardEntity[tileCoord][i].Release();
                boardEntity[tileCoord][i] = null;
                boardEntity.BoardEffect.ApplyEffect(tileCoord, color);
                boardEntity[tileCoord][i] = null;
            }
        }
    }

    public IEnumerator IECancelInsert(RingHolder ringHolder = null)
    {
        if (!ringHolder)
        {
            ringHolder = ringSpawner.GetRingHolder();
        }
        yield return IEMoveTo(ringSpawner.transform.position, ringHolder.transform, 0.2f);
        RingHolder.IsLock = false;
    }

    private static IEnumerator IEMoveTo(Vector3 destination, Transform transform, float time)
    {
        var t = 0f;
        var startPoint = transform.position;
        while (t < time)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPoint, destination, t / time);
            yield return null;
        }

        transform.position = destination;
    }

    public void CreateRing(RingSize ringSize, Color color, Vector2Int coordinate)
    {
        var ringEntity = PoolManager.Instance[((RingSize)ringSize).ToString()].Get<RingEntity>(boardEntity.transform);
        ringEntity.transform.position = boardEntity[coordinate].transform.position;
        ringEntity.Color = color;
        var tile = board[coordinate.x, coordinate.y];
        tile[ringSize] = color;
        boardEntity[coordinate][ringSize] = ringEntity;
    }

    protected virtual void ActionCombo(int comboCount)
    {
        if (comboCount <= 2) return;
        OnComboComplete?.Invoke(comboCount);
        PhoneManager.Instance.Viberate(100);
        var combo = PoolManager.Instance["Combo"].Get<Combo>();
        combo.Count = comboCount;
        combo.Animation(comboContainer.position);
        switch (comboCount)
        {
            case 3:
                AudioManager.Instance.PlaySound(EventSound.Combo0);
                break;
            case 4:
                AudioManager.Instance.PlaySound(EventSound.Combo0);
                break;
            case 5:
                AudioManager.Instance.PlaySound(EventSound.Combo1);
                break;
            case 6:
                AudioManager.Instance.PlaySound(EventSound.Combo2);
                break;
            default:
                AudioManager.Instance.PlaySound(EventSound.Combo3);
                break;
        }
    }

    protected virtual void SpawnRing()
    {
        AudioManager.Instance.PlaySound(EventSound.SpawnRing);
        ringSpawner.SpawnNew();
    }

    protected virtual void OnLose()
    {
        if (gameState != GameState.Playing) return;
        gameState = GameState.Lose;
        LoseCount++;
        OnLoseGame?.Invoke();
    }

    public void CheckTarget()
    {
        if (!UIInGame.Instance.ChallengeManager.ChallengeIsComplete) return;
        OnGameWin();
    }

    private void OnGameWin()
    {
        if(gameState != GameState.Playing) return;
        gameState = GameState.Win;
        RingsRemain = board.GetRingsAmount();
        OnWinGame?.Invoke();
    }
}

public interface ILoseEvent
{
    void OnGameLose();
}

public enum GameState
{
    Playing, Win, Lose
}

public interface IStartEvent
{
    void OnGameStart();
}

public class TimeCount : MonoBehaviour, ILoseEvent, IStartEvent, Message.ICallback
{
    private void OnEnable()
    {
        Message.Use<Type>().With(this).Sub(typeof(IStartEvent), typeof(ILoseEvent));
    }

    private void OnDisable()
    {
        Message.Use<Type>().With(this).UnSub(typeof(IStartEvent), typeof(ILoseEvent));
    }

    public void OnGameLose()
    {
    }

    public void OnGameStart()
    {
    }
}