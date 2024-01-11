using UnityEngine;

public class BoardMask : MonoBehaviour
{
    [SerializeField] private BoardBaseEntity boardEntity;

    private void Start()
    {
        transform.position = boardEntity.transform.position;
        transform.localScale = new Vector3(boardEntity.Size.x * boardEntity.TileSize.x + 0.05f, boardEntity.Size.y * boardEntity.TileSize.y + 0.05f, 0);
    }
}
