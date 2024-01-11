using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Puzzle.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class GameLoader : PersistentSingleton<GameLoader>
{
    [SerializeField] private GamePlayModeData[] gamePlayModes;
    private readonly Dictionary<ModeGameType, GamePlayModeData> _gamePlayDictionary = new();
    private List<GameObject> gameObjects = new();
    private IEnumerator _countTimeAdsBreak;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        Initialize();
    }

    private void Initialize()
    {
        for (var i = 0; i < gamePlayModes.Length; i++)
        {     
            _gamePlayDictionary.Add(gamePlayModes[i].modeGameType,gamePlayModes[i]);
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="level">[0, GameConstant.MAX_LEVEL]</param>
    public virtual void Load(int level = -1, GameObject popup = null)
    {
        if (popup != null) Hub.Hide(popup).Play();

        StartCoroutine(IELoad(level));
    }
    
    public virtual void Load(Level data)
    {
        StartCoroutine(IELoad(GameDataManager.MaxLevelUnlock, data));
    }

    private IEnumerator IELoad(int level = -1, Level data = null)
    {
        var asyncLoad = SceneManager.LoadSceneAsync("ThemeWood");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        if (level == -1) level = GameDataManager.MaxLevelUnlock;

        QuitGamePlay();
        
        data ??= GetNextLevelInfo(level);
        // GameManager must be initialized to UIInGame to get data from GameManager
        var gamePlayMode = _gamePlayDictionary[data.ModeGameType];
        var cacheGameManager = Instantiate(gamePlayMode.gameManager, transform);
        var cacheUiInGame = Instantiate(gamePlayMode.uiInGame, transform);
            
        gameObjects.Add(cacheUiInGame.gameObject);
        gameObjects.Add(cacheGameManager.gameObject);

        cacheGameManager.SetUp(data, GetSkin(GameDataManager.CurrentSkin));
        cacheUiInGame.SetUp(data, GameDataManager.LevelToShow(level + 1));

        _countTimeAdsBreak = AdsManager.Instance.StartCountAdsBreak();
        StartCoroutine(_countTimeAdsBreak);
    }

    public static Level GetNextLevelInfo(int currentLevel)
    {
        var realLevel = currentLevel;
        if (currentLevel >= GameConstant.MAX_LEVEL)
            realLevel = GameConstant.GAP_LEVEL_TUTORIAL + currentLevel % (GameConstant.MAX_LEVEL - GameConstant.GAP_LEVEL_TUTORIAL);

        if (GameDataManager.IsLevelBonus(realLevel + 1))
        {
            var bonusLevel = GameDataManager.CountLevelBonus(realLevel + 1);
            return Resources.Load<Level>($"Level/Level Bonus {bonusLevel}");
        }

        return Resources.Load<Level>($"Level/Level {GameDataManager.LevelToShow(realLevel + 1)}");
    }

    private static Skin GetSkin(int skinID)
    {
        return GameDataManager.GetSkinByID(skinID);
    }

    public void Load(ModeGameType modeGameType = ModeGameType.Endless, GameObject popup = null)
    {
        if (modeGameType == ModeGameType.Normal) return;
        if (popup != null) Hub.Hide(popup).Play();
        StartCoroutine(IELoad(modeGameType));
    }
    
    private IEnumerator IELoad(ModeGameType modeGameType = ModeGameType.Endless)
    {
        var asyncLoad = SceneManager.LoadSceneAsync("ThemeWood");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        QuitGamePlay();
        var level = GetResourceName(modeGameType);
        
        var data = Resources.Load<Level>($"Level/Level " + level);
        data.ResetColor(3); //todo delete
        // GameManager must be initialized to UIInGame to get data from GameManager
        var gamePlayMode = _gamePlayDictionary[modeGameType];
        var cacheGameManager = Instantiate(gamePlayMode.gameManager, transform);
        var cacheUiInGame = Instantiate(gamePlayMode.uiInGame, transform);

        cacheUiInGame.SetUp(data, level.ToString(CultureInfo.CurrentCulture));
        cacheGameManager.SetUp(data, GetSkin(GameDataManager.CurrentSkin));

        gameObjects.Add(cacheUiInGame.gameObject);
        gameObjects.Add(cacheGameManager.gameObject);
        _countTimeAdsBreak = AdsManager.Instance.StartCountAdsBreak();
        StartCoroutine(_countTimeAdsBreak);
    }

    private string GetResourceName(ModeGameType modeGameType) => modeGameType switch
    {
        ModeGameType.Endless => GameConst.EndlessLevelResource,
        ModeGameType.AdvancedEndless => GameConst.AdvancedEndlessLevelResource,
        _ => throw new ArgumentOutOfRangeException(nameof(modeGameType), modeGameType, null)
    };
    
    public void LoadHome()
    {
        QuitGamePlay();
        SceneManager.LoadScene("Home");
    }
    
    private void QuitGamePlay()
    {
        foreach (var go in gameObjects) DestroyImmediate(go);
        gameObjects.Clear();
        
        if(_countTimeAdsBreak != null) StopCoroutine(_countTimeAdsBreak);
    }

    public void StopCountTimeAdsBreak()
    {
        if (_countTimeAdsBreak != null) StopCoroutine (_countTimeAdsBreak);
    }
    
}

public enum ModeGameType
{
    Normal,
    Endless,
    AdvancedEndless,
    Bonus,
    Survivor,
    Hard,
    SuperHard
}

[Serializable]
public class GamePlayModeData
{
    public ModeGameType modeGameType;
    public GameManager gameManager;
    public UIInGame uiInGame;
}