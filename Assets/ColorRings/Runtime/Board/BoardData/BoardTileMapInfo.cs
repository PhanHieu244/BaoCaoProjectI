using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardTileMapInfo : MonoBehaviour
{
    [field: SerializeField] public Tilemap Board { get; private set; }
    [field: SerializeField] public Tilemap RandomLockTile { get; private set; }
    [field: SerializeField] public Tilemap AbsoluteLockTile { get; private set; }
}
