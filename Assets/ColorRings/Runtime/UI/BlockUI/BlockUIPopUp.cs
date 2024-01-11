using Puzzle.UI;
using UnityEngine;

public class BlockUIPopUp : PopUpContent
{
    private int _blockCount;

    private void Awake()
    {
        transform.SetParent(null);
    }

    private void OnEnable()
    {
        var canvas = GetComponent<Canvas>();
        if(canvas == null) return;
        canvas.sortingLayerName = "UI";
        canvas.overrideSorting = true;
        canvas.sortingOrder = 100;
    }

    public void Block()
    {
        _blockCount++;
        DevLog.Log("Block Count",_blockCount);
        if (_blockCount <= 0) return;
        gameObject.SetActive(true);
    }
        
    public void UnBlock()
    {
        _blockCount--;
        DevLog.Log("UnBlock Count",_blockCount);
        if (_blockCount > 0) return;
        gameObject.SetActive(false);
    }
}