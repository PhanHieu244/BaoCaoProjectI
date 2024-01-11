using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public interface ITutorial
{
    public void InitializeBoard(GameManager gameManager);
    public void StartTutorial();
    bool IsSpawn();
    public Level.TileData GetPattern();

    public int GetSizeCoord();

    public int SizeStep();

    public Vector2Int GetTargetCoord();

    public bool IsDoneTutorial();

    public bool IsDoneAStep();

    public bool HaveNextStep();

    public Vector2 GetPositionClick();

    public void IsClicked();

    public bool IsNormalTutorial();

    public bool IsItemTutorial();

    public ItemType GetItemType();
}

[Serializable]
public class Tutorial : ITutorial
{
    [SerializeField] private Level.BoardData boardData;
    public List<Level.TileData> _qC = new List<Level.TileData>(3);
    public Vector2Int[] targetCoord = new Vector2Int[5];
    public int[] sizeCoord = new int[5];
    private int currentStep = 0;
    private int indexRing = 3;


    public void InitializeBoard(GameManager gameManager)
    {
        // init and return board;
        currentStep = 0;
        var tiles = boardData.tiles;
        for (int i = 0; i < tiles.Length; i++)
        {
            int rol = i / 3;
            int col = i % 3;
            var colors = tiles[i].colors;
            for (int j = 0; j < colors.Length; j++)
            {
                if (colors[j] != Color.NONE)
                {
                    gameManager.CreateRing((RingSize)j, colors[j], new Vector2Int(col, rol));
                }
            }
        }
    }

    public void StartTutorial()
    {
        indexRing = _qC.Count;

        TutorialManager.Instance.StartTutorial(this);
    }

    public bool IsSpawn() => indexRing >= 0;

    public Level.TileData GetPattern()
    {
        indexRing--;
        if (indexRing <= 0) return _qC[0];
        else
        {
            return _qC[indexRing];
        }
    }

    public int SizeStep()
    {
        return sizeCoord.Length;
    }

    public int GetSizeCoord()
    {
        if (currentStep > sizeCoord.Length - 1) return -1;
        return sizeCoord[currentStep];
    }

    public Vector2Int GetTargetCoord()
    {
        return targetCoord[currentStep];
    }

    public bool IsDoneTutorial()
    {
        return currentStep > targetCoord.Length - 1;
    }

    public bool IsDoneAStep()
    {
        var pos = GameManager.Instance.RingSpawner.GetRingHolderPosition();
        var coordUp = GameManager.Instance.BoardEntity.InputCoordinateAvailable(pos);
        if (coordUp == targetCoord[currentStep])
        {
            if (currentStep <= targetCoord.Length - 1) currentStep++;
            return true;
        }
        return false;
    }

    public bool HaveNextStep() => currentStep < targetCoord.Length;

    public Vector2 GetPositionClick()
    {
        return Vector2.positiveInfinity;
    }

    public bool IsNormalTutorial()
    {
        return true;
    }

    public bool IsItemTutorial()
    {
        return false;
    }

    public void IsClicked()
    {
        return;
    }

    public ItemType GetItemType()
    {
        return ItemType.None;
    }

    public bool isItemTutorial(ItemType itemType)
    {
        throw new NotImplementedException();
    }
}


public abstract class ItemTutorial : ITutorial
{
    [SerializeField] private Level.BoardData boardData;
    protected List<Vector2> positionClicks = new List<Vector2>();
    public List<Vector2Int> targetCoords;
    protected int indexClick;
    public Level.TileData GetPattern()
    {
        throw new NotImplementedException();
    }

    public void InitializeBoard(GameManager gameManager)
    {
        // init and return board;
        var tiles = boardData.tiles;
        for (int i = 0; i < tiles.Length; i++)
        {
            int rol = i / 3;
            int col = i % 3;
            var colors = tiles[i].colors;
            for (int j = 0; j < colors.Length; j++)
            {
                if (colors[j] != Color.NONE)
                {
                    gameManager.CreateRing((RingSize)j, colors[j], new Vector2Int(col, rol));
                }
            }
        }

        return;
    }

    public virtual void StartTutorial()
    {
        RingHolder.IsLock = true;
        for (int i = 0; i < targetCoords.Count; i++)
        {
            Vector2Int coordinate = targetCoords[i];
            positionClicks.Add(GameManager.Instance.BoardEntity[coordinate].transform.position);
        }
        
        positionClicks[^1] = UIInGame.Instance.GetItemPos(GetItemType());
        
        indexClick = positionClicks.Count - 1;
        
        TutorialManager.Instance.StartTutorialItem(this);

    }
    
    public int SizeStep()
    {
        return 0;
    }
    
    public int GetSizeCoord()
    {
        return -1;
    }

    public bool IsSpawn()
    {
        return false;
    }

    public Vector2Int GetTargetCoord()
    {
        return Vector2Int.zero;
    }


    public Vector2 GetPositionClick()
    {
        if (indexClick < 0) return Vector2.positiveInfinity;
        return positionClicks[indexClick];
    }

    public bool IsDoneTutorial()
    {
        if (TutorialManager.IsTurotialDone)  return true;
        if (indexClick < 0)
        {
            return true;
        }
        return false;

    }

    public bool IsDoneAStep()
    {
        return true;
    }

    public bool IsNormalTutorial()
    {
        return false;
    }

    public bool IsItemTutorial()
    {
        return true;
    }

    public void IsClicked()
    {
        indexClick--;
    }

    public virtual ItemType GetItemType()
    {
        return ItemType.Hammer;
    }

    public bool IsItemTutorial(ItemType itemType)
    {
        if (IsDoneTutorial()) return false;
        if (itemType != ItemType.Hammer) return false;
        return true;
    }

    public bool HaveNextStep()
    {
        return false;
    }
}
[Serializable]
public class HammerTutorial : ItemTutorial
{

    override
    public ItemType GetItemType()
    {
        return ItemType.Hammer;
    }
}

[Serializable]
public class DestroyAColTutorial : ItemTutorial
{
    override
    public ItemType GetItemType()
    {
        return ItemType.DestroyACol;
    }
}

[Serializable]
public class DestroyARowTutorial : ItemTutorial
{
    override
    public ItemType GetItemType()
    {
        return ItemType.DestroyARow;
    }
}

[Serializable]
public class SwapTutorial : ItemTutorial
{

    override
    public ItemType GetItemType()
    {
        return ItemType.Swap;
    }
}
