using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TileEntity : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private IEntity[] _colors = new IEntity[Board.Depth];


    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void SetupClassicTile(bool isClassicBoard)
    {
        _renderer.enabled = !isClassicBoard;
    }
    
    public IEntity this[RingSize ringSize]
    {
        get => _colors[(int)ringSize];
        set => _colors[(int)ringSize] = value;
    }
    
    public IEntity this[int size]
    {
        get => _colors[size];
        set => _colors[size] = value;
    }
}