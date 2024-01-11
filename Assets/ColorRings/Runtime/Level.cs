using System;
using System.Collections.Generic;
using System.Linq;
using ColorRings.Runtime.Board.Entity;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Level", fileName = "Level 1", order = 0)]
public class Level : ScriptableObject
{
    [SubclassSelector, SerializeReference] private ILevelUpStrategy _levelUpStrategy;
    [SubclassSelector, SerializeReference] private IBoardShapeInLevel _boardShapeInLevel;
    [SerializeField] private ModeGameType modeGameType = ModeGameType.Normal;
    [SerializeReference, SubclassSelector] public IChallengeData[] challenges;
    public PowerUpType reward;
    public int rewardAmount;

    public Pattern[] patterns;

    [SerializeReference, SubclassSelector] public ITutorial tutorialMod;

    public List<SizePattern> sizePatterns;
    public List<ColorPattern> colorPatterns;

    public ILevelUpStrategy levelUpStrategy => _levelUpStrategy;

    public BoardShape BoardShape => _boardShapeInLevel?.BoardShape;
    public virtual ModeGameType ModeGameType => modeGameType;

    [ContextMenu("create pattern from color and size")]
    public void CreatePattern()
    {
        var pt = new List<Pattern>();
        var totalColor = colorPatterns.Sum(c => c.ratio);
        var totalSize = sizePatterns.Sum(s => s.ratio);

        foreach (var sizePattern in sizePatterns)
        {
            if (sizePattern.sizes.Length > 1)
            {
                var queryPattern = new Color[Board.Depth];

                Query(sizePattern.sizes.Length - 1, queryPattern, sizePattern.ratio / totalSize);

                void Query(int depth, Color[] pattern, float ratio)
                {
                    if (depth == 0)
                    {
                        foreach (var colorPattern in colorPatterns)
                        {
                            var queryPatternCopy = pattern.ToArray();
                            queryPatternCopy[(int)sizePattern.sizes[depth]] = colorPattern.color;
                            pt.Add(new Pattern
                            {
                                ratio = colorPattern.ratio * ratio / totalColor,
                                value = queryPatternCopy,
                            });
                        }
                    }
                    else
                    {
                        foreach (var colorPattern in colorPatterns)
                        {
                            var queryPatternCopy = pattern.ToArray();
                            queryPatternCopy[(int)sizePattern.sizes[depth]] = colorPattern.color;
                            Query(depth - 1, queryPatternCopy, ratio * (colorPattern.ratio / totalColor));
                        }
                    }
                }
            }
            else
            {
                foreach (var colorPattern in colorPatterns)
                {
                    var pattern = new Pattern
                    {
                        ratio = sizePattern.ratio * colorPattern.ratio / (totalColor * totalSize),
                        value = new Color[Board.Depth]
                    };
                    pattern.value[(int)sizePattern.sizes[0]] = colorPattern.color;
                    pt.Add(pattern);
                }
            }
        }

        patterns = pt.ToArray();

        foreach (var p in patterns)
        {
            p.ratio *= totalColor * totalSize;
        }
    }

    public void IncreaseColor()
    {
        var maxColor = colorPatterns[^1].color;
        var currentRatio = colorPatterns[^1].ratio;
        var c_pattern = new ColorPattern();
        c_pattern.ratio = currentRatio;
        if (maxColor + 1 < Color.COUNT)
        {
            c_pattern.color = (Color)(maxColor + 1);
            colorPatterns.Add(c_pattern);
            CreatePattern();
        }
        else return;
    }

    public void ResetColor(int ColorAmount)
    {
        var oldRatio = colorPatterns[^1].ratio;
        colorPatterns.Clear();
        for (int i = 1; i <= ColorAmount; i++)
        {
            var c_pattern = new ColorPattern();
            c_pattern.ratio = oldRatio;
            c_pattern.color = (Color)i;
            colorPatterns.Add(c_pattern);
        }

        CreatePattern();
    }

    [Serializable]
    public class BoardData
    {
        public TileData[] tiles = new TileData[9];
    }

    [Serializable]

    public class TileData
    {
        public Color[] colors;
    }
}

[Serializable]
public class SizePattern
{
    public RingSize[] sizes;
    public float ratio;
}

[Serializable]
public class ColorPattern
{
    public Color color;
    public float ratio;
}

[Serializable]
public class Pattern
{
    public Color[] value;
    public float ratio;
}

