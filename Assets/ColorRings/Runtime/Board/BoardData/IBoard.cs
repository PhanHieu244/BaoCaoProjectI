using System.Collections.Generic;
using UnityEngine;

public interface IBoard
{ 
    int Width { get; }
    int Height { get; }
    int DEPTH => Board.Depth;
    Tile this[int x, int y] { get; set; }
    Tile this[Vector2Int coordinate] { get; set; }
    bool HasTile(int x, int y);
    bool HasTile(int x, int y, out bool isValidRange);
    bool HasRing(int x, int y);
    void LockTile(Vector2Int coord);
    void UnlockLockTile(Vector2Int coord);
    bool IsLock(Vector2Int coord);
    int GetRingsAmount();
    SizePattern GetAvailablePattern(List<SizePattern> sizePatterns, Color[] patternInHolder = null);
    IEnumerable<SizePattern> AvailablePattern(SizePattern[] sizePatterns);

}