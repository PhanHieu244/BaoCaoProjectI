using Unity.Mathematics;
using UnityEngine;

public class BoardEntity : BoardBaseEntity
{
    private void Awake()
    {
        // create data
        DistanceCheck = Mathf.Min(TileSize.x, TileSize.y) / 2;

        // create render board and cache position tiles
        tileEntities = new TileEntity[Size.x, Size.y];
        var startPoint = new Vector2(-Size.x * TileSize.x, -Size.y * TileSize.y) * 0.5f + (Vector2)transform.position;
        for (var x = 0; x < Size.x; x++)
        {
            for (var y = 0; y < Size.y; y++)
            {
                var pos = startPoint + new Vector2((x + 0.5f) * TileSize.x, (y + 0.5f) * TileSize.y);
                tileEntities[x, y] = Instantiate(tileEntityPrefab, pos, quaternion.identity, transform);
            }
        }
    }

    private void OnDrawGizmos()
    {
        var startPoint = new Vector2(-Size.x * TileSize.x, -Size.y * TileSize.y) * 0.5f + (Vector2)transform.position;
        for (var x = 0; x < Size.x; x++)
        {
            for (var y = 0; y < Size.y; y++)
            {
                Gizmos.DrawWireCube(startPoint + new Vector2((x + 0.5f) * TileSize.x, (y + 0.5f) * TileSize.y), TileSize);
            }
        }
    }

    public override Vector2Int? InputCoordinateAvailable(Vector2 current)
    {
        for (var x = 0; x < Size.x; x++)
        {
            for (var y = 0; y < Size.y; y++)
            {
                if (DistanceCheck > Vector2.Distance(current, tileEntities[x, y].transform.position))
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return null;
    }

    public override void InitBoardData(out IBoard board)
    {
        board = new Board(Size);
    }
}