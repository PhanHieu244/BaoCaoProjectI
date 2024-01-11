using UnityEngine;

public abstract class BoardBaseEntity : MonoBehaviour
{
    [SerializeField] protected TileEntity tileEntityPrefab;
    [field: SubclassSelector, SerializeReference]
    private IBoardEffect _boardEffect = new ClassicBoardEffect();
    [field: SerializeField] public Vector2Int Size { get; protected set; }
    [field: SerializeField] public Vector2 TileSize { get; protected set; }
    protected TileEntity[,] tileEntities;
    

    protected float DistanceCheck { get; set; }
    public IBoardEffect BoardEffect => _boardEffect;

    public TileEntity this[Vector2Int coordinate]
    {
        get => tileEntities[coordinate.x, coordinate.y];
        set => tileEntities[coordinate.x, coordinate.y] = value;
    }
    
    public TileEntity this[int x, int y] => tileEntities[x, y];

    public virtual BoardType BoardType => BoardType.Classic;

    public abstract Vector2Int? InputCoordinateAvailable(Vector2 current);

    public abstract void InitBoardData(out IBoard board);
    protected virtual void Start()
    {
        //if(Camera.main is not null) Camera.main.orthographicSize = cameraProjectionSize;
        BoardEffect.Initialize(this);
    }

    public void Apply(Match match, Color color)
    {
        for (var i = 0; i < match.Size.Length; i++)
        {
            this[match.Coordinate][match.Size[i]].Release();
            this[match.Coordinate][match.Size[i]] = null;
        }

        BoardEffect.ApplyEffect(match.Coordinate, color);
    }
    
    public Vector2 GetBoardPos(Vector2Int boardPos)
    {
        return this[boardPos].transform.position;
    }
}

public enum BoardType
{
    Classic,
    RotateBoard
}