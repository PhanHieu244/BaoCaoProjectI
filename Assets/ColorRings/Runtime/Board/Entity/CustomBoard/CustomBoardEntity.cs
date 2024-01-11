using System.Collections.Generic;
using ColorRings.Runtime.Board.Entity;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;

[DefaultExecutionOrder(-99)]
[RequireComponent(typeof(BaseBoardShapeManager))]
public class CustomBoardEntity : BoardBaseEntity
{
    [field: SerializeField] public BoardShape BoardShape { get; private set; }
    [SerializeField] private LockTile lockTilePrefabs;
    private List<Vector2Int> _availableTiles;
    private List<Vector2Int> _lockTiles;
    private bool _hasLockTile;
    private bool _isClassicBoard;
    public override BoardType BoardType => BoardShape.RotateZ == 0 ? BoardType.Classic : BoardType.RotateBoard;

    protected override void Start()
    {
        base.Start();
        InitVisualBoard();
    }

    protected virtual void GetBoardShape()
    {
        var boardShapeManager = GetComponent<BaseBoardShapeManager>();
        _hasLockTile = boardShapeManager.HasLock();
        BoardShape = boardShapeManager.GetBoardShape();
        _isClassicBoard = BoardShape.IsClassicBoard();
    }

    private void InitVisualBoard()
    {
        InitShapeData();
        // create data
        DistanceCheck = Mathf.Min(TileSize.x, TileSize.y) / 2;
        
        // create render board and cache position tiles
        tileEntities = new TileEntity[Size.x, Size.y];
        var startPoint = new Vector2(-Size.x * TileSize.x, -Size.y * TileSize.y) * 0.5f + (Vector2)transform.position;
        foreach (var availableTile in _availableTiles)
        {
            var pos = startPoint + new Vector2((availableTile.x + 0.5f) * TileSize.x, (availableTile.y + 0.5f) * TileSize.y);
            tileEntities[availableTile.x, availableTile.y] = Instantiate(tileEntityPrefab, pos, quaternion.identity, transform);
            tileEntities[availableTile.x, availableTile.y].SetupClassicTile(_isClassicBoard);
        }

        foreach (var tile in _lockTiles)
        {
            var pos = startPoint + new Vector2((tile.x + 0.5f) * TileSize.x, (tile.y + 0.5f) * TileSize.y);
            var lockTileEntity = Instantiate(lockTilePrefabs, pos, quaternion.identity, transform);
            lockTileEntity.Setup(this, tile);
        }
    }
    
    private void InitShapeData()
    {
        GetBoardShape();
        Size = BoardShape.Size;
        _availableTiles = BoardShape.AvailableTiles;
        _lockTiles = new List<Vector2Int>();
        if (!_hasLockTile) return;
        SettingLockTiles(BoardShape.GetLockTiles());
    }

    private void SettingLockTiles(List<Vector2Int> lockTilesInBoardShape)
    {
        if (lockTilesInBoardShape is null) return;
        foreach (var lockTile in lockTilesInBoardShape)
        {
            _lockTiles.Add(lockTile);
        }
    }

    private void OnDrawGizmos()
    {
        var startPoint = new Vector2(-BoardShape.Size.x * TileSize.x, -BoardShape.Size.y * TileSize.y) * 0.5f + (Vector2)transform.position;
        for (var x = 0; x < BoardShape.Size.x; x++)
        {
            for (var y = 0; y < BoardShape.Size.y; y++)
            {
                Gizmos.DrawWireCube(startPoint + new Vector2((x + 0.5f) * TileSize.x, (y + 0.5f) * TileSize.y), TileSize);
            }
        }
        foreach (var availableTile in BoardShape.AvailableTiles)
        {
            Gizmos.color = UnityEngine.Color.green;
            Gizmos.DrawWireCube(startPoint + new Vector2((availableTile.x + 0.5f) * TileSize.x,
                (availableTile.y + 0.5f) * TileSize.y), TileSize);;
        }
    }

    public override Vector2Int? InputCoordinateAvailable(Vector2 current)
    {
        int x, y;
        foreach (var availableTile in _availableTiles)
        {
            x = availableTile.x;
            y = availableTile.y;
            if (!(DistanceCheck > Vector2.Distance(current, tileEntities[x, y].transform.position))) continue;
            if(!_hasLockTile) return new Vector2Int(x, y);
            if (_lockTiles.Exists(tile => tile.x == x && tile.y == y)) return null;
            return new Vector2Int(x, y);
        }
        return null;
    }

    public void UnlockTile(Vector2Int coord)
    {
        var canRemove = _lockTiles.Remove(coord);
        if (!canRemove) return;
        GameManager.Instance.Board.UnlockLockTile(coord);
    }

    public override void InitBoardData(out IBoard board)
    {
        board = new CustomBoard(Size, _availableTiles);
        foreach (var lockTile in _lockTiles)
        {
            board.LockTile(lockTile);
        }
    }
}
