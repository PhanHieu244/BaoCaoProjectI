using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;


namespace ColorRings.Runtime.Board.Entity
{
    [CreateAssetMenu(fileName = "BoardShape", menuName = "Scriptable Objects/BoardShape")]
    public class BoardShape : ScriptableObject
    {
        [field: Header("Available Tiles Data")]
        [field: SerializeField, ReadOnly] public List<Vector2Int> AvailableTiles { get; private set; }
        [field: SerializeField, ReadOnly] public Vector2Int Size { get; private set; }
        [field: SerializeField, ReadOnly] public float RotateZ { get; private set; }
        [field: SerializeField, ReadOnly] public float Height { get; private set; }
        [field: SerializeField, ReadOnly] public float DeltaRotatePos { get; private set; }
        [field: SerializeField, ReadOnly] public float MaxSize { get; private set; }
        [field: Header("Locked Tiles Data")]
        [SerializeField, ReadOnly] private List<Vector2Int> randomLockTiles;
        [SerializeField, ReadOnly] private List<Vector2Int> absoluteLockTiles;
        [SerializeReference, SubclassSelector] private IRandomLockTileStrategy _lockTileStrategy;
        public List<Vector2Int> GetLockTiles() => _lockTileStrategy.GetLockTilesInGame(randomLockTiles, absoluteLockTiles);

        public bool IsClassicBoard()
        {
            return Size.x == 3 && Size.y == 3 && RotateZ == 0;
        }
        

#if UNITY_EDITOR
        [Header("Setting In Editor")]
        [SerializeField] private string nameToLoadTileMap;

        [SerializeField] private Tilemap tileMapAvailable, tilemapRandomLock, tilemapAbsoluteLock;

        private void GetDataByName(string name)
        {
            if (name.Equals(""))
            {
                GetDataByName();
            }

            var cache = nameToLoadTileMap;
            nameToLoadTileMap = name;
            GetTileMap();
            GetBoardShape();
            nameToLoadTileMap = cache;
        }
        
        [Button("Auto Get Data By Name")]
        private void GetDataByName()
        {
            GetTileMap();
            GetBoardShape();
            GetMaxSize();
            Save();
        }
        
        private void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
            AssetDatabase.Refresh();
        }
        
        
        private void GetTileMap()
        {
            var path = $"Assets/ColorRings/Editor/TileMap/CustomBoardShape {nameToLoadTileMap}.prefab";
            var infoMap = AssetDatabase.LoadAssetAtPath<BoardTileMapInfo>(path);
            tileMapAvailable = infoMap.Board;
            tilemapRandomLock = infoMap.RandomLockTile;
            tilemapAbsoluteLock = infoMap.AbsoluteLockTile;
        }
        
        private void GetBoardShape()
        {
            float z = (int)tileMapAvailable.transform.rotation.eulerAngles.z;
            if (z != 0 && Math.Abs(z - 45) >= 0.1f && z < 0)
            {
                DevLog.LogError($"{z}: rotate Z must be 0 or 45!!!!!!");
                return;
            }

            RotateZ = z == 0? 0 : 45f;
            AvailableTiles = new List<Vector2Int>();
            tileMapAvailable.CompressBounds();
            Size = (Vector2Int) tileMapAvailable.size;
            var minBounds = tileMapAvailable.cellBounds.min;
            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    if (tileMapAvailable.HasTile(new Vector3Int(x + minBounds.x, y + minBounds.y, 0)))
                    {
                        AvailableTiles.Add(new Vector2Int(x, y));
                    }
                }
            }
            
            //get LockTile
            if (tilemapRandomLock is null)
            {
                DevLog.LogError("random tile map lock is null");
                tilemapRandomLock = new Tilemap();
            }
            if (tilemapAbsoluteLock is null)
            {
                DevLog.LogError("absolute tile map lock is null");
                tilemapAbsoluteLock = new Tilemap();
            }

            randomLockTiles = new List<Vector2Int>();
            absoluteLockTiles = new List<Vector2Int>();
            foreach (var availableTile in AvailableTiles)
            {
                var coord = new Vector3Int(availableTile.x + minBounds.x, availableTile.y + minBounds.y, 0);
                if (tilemapRandomLock.HasTile(coord))
                {
                    randomLockTiles.Add(availableTile);
                    continue;
                }

                if (tilemapAbsoluteLock.HasTile(coord))
                {
                    absoluteLockTiles.Add(availableTile);
                }
            }
        }
        
        private void GetMaxSize()
        {
            DeltaRotatePos = 0;
            Height = GetHeight();
            MaxSize = Mathf.Max(Height, GetWeight());
        }

        private float GetHeight()
        {
            if (RotateZ == 0) return Size.y;
            var above = 0f;
            var below = 0f;
            foreach (var tile in AvailableTiles)
            {
                var distance = DistanceToDiagonal(tile.x, tile.y, Size.x - 1, Size.y - 1, out var isAbove);
                if(isAbove) above = Mathf.Max(distance, above);
                else below = Mathf.Max(distance, below);
            }
            DevLog.Log("above",above);
            DevLog.Log("below",below);
            DeltaRotatePos = above - (above + below) / 2;
            return above + below + Mathf.Sqrt(2);
        }
        
        static float DistanceToDiagonal(float x1, float y1, float x0, float y0, out bool isAbove)
        {
            return CalculateDistanceToLine(x1, y1, 0, x0, y0, 0,out isAbove);
        }
        
        
        static float CalculateDistanceToLine(float x0, float y0, float x1, float y1, float x2, float y2, out bool isAbove)
        {
            // Calculate the coefficients of the line equation
            double A = y2 - y1;
            double B = x1 - x2;
            double C = x2 * y1 - x1 * y2;
            isAbove = A * x0 + B * y0 + C > 0;
            // Calculate the distance from the point to the line
            double distance = Math.Abs(A * x0 + B * y0 + C) / Math.Sqrt(A * A + B * B);

            return (float) distance;
        }
        
        static float DistanceToLine(float x1, float y1, float x0, float y0)
        {
            return CalculateDistanceToLine(x1, y1, 0, 0, x0, y0,out var isAbove); 
        }

        private float GetWeight()
        {
            if (RotateZ == 0) return Size.x;
            var above = 0f;
            var below = 0f;
            foreach (var tile in AvailableTiles)
            {
                var distance = DistanceToLine(tile.x, tile.y, Size.x - 1, Size.y - 1);
                above = Mathf.Max(distance, above);
                below = Mathf.Max(distance, below);
            }

            return above + below + Mathf.Sqrt(2);
        }
#endif
    }
}

public interface IRandomLockTileStrategy
{
    List<Vector2Int> GetLockTilesInGame(List<Vector2Int> randomLockTiles, List<Vector2Int> absoluteLockTiles);
}

[Serializable]
public class GetAllLockTiles : IRandomLockTileStrategy
{
    public List<Vector2Int> GetLockTilesInGame(List<Vector2Int> randomLockTiles, List<Vector2Int> absoluteLockTiles)
    {

        var lockList = randomLockTiles?.ToList() ?? new List<Vector2Int>();
        if (absoluteLockTiles is not null)
        {
            lockList.AddRange(absoluteLockTiles);
        }
        return lockList;
    }
}

[Serializable]
public abstract class GetRandomInLockTiles : IRandomLockTileStrategy
{
    protected List<Vector2Int> GetLockTilesInGame(List<Vector2Int> randomLockTiles, int amountRandom)
    {
        var maxSize = randomLockTiles.Count;
        if (amountRandom >= maxSize) return randomLockTiles.ToList();
        var checkID = new bool[maxSize];
        var listLock = new List<Vector2Int>();
        while (listLock.Count < amountRandom)
        {
            var randomID = Random.Range(0, maxSize);
            if(checkID[randomID]) continue;
            listLock.Add(randomLockTiles[randomID]);
            checkID[randomID] = true;
        }

        return listLock;
    }

    public abstract List<Vector2Int> GetLockTilesInGame(List<Vector2Int> randomLockTiles, List<Vector2Int> absoluteLockTiles);
}

[Serializable]
public class GetLockTilesByAmount : GetRandomInLockTiles
{
    [SerializeField] private int amount;
    public override List<Vector2Int> GetLockTilesInGame(List<Vector2Int> randomLockTiles, List<Vector2Int> absoluteLockTiles)
    {
        var lockList = GetLockTilesInGame(randomLockTiles, amount) ?? new List<Vector2Int>();
        if (absoluteLockTiles is not null)
        {
            lockList.AddRange(absoluteLockTiles);
        }
        return lockList;
    }
}

[Serializable]
public class GetLockTilesByRange : GetRandomInLockTiles
{
    [SerializeField] private int min, max;

    public override List<Vector2Int> GetLockTilesInGame(List<Vector2Int> randomLockTiles, List<Vector2Int> absoluteLockTiles)
    {
        var randomAmount = Random.Range(min, max + 1);
        var lockList = GetLockTilesInGame(randomLockTiles, randomAmount) ?? new List<Vector2Int>();
        if (absoluteLockTiles is not null)
        {
            lockList.AddRange(absoluteLockTiles);
        }
        return lockList;
    }
}