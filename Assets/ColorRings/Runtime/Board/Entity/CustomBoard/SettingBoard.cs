using System;
using ColorRings.Runtime.Board.Entity;
using UnityEngine;

[RequireComponent(typeof(CustomBoardEntity))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SpriteMask))]
public class SettingBoard : MonoBehaviour
{
    [SerializeField] private RingSpawner ringSpawner;
    [SerializeField] private CustomBoardEntity customBoardEntity;
    [SerializeField] private Transform comboContainer, gate;

    private SpriteRenderer _boardRenderer;
    private SpriteMask _boardMask;
    
    public const float ClassicCameraSize = 3.4f; //for 3x3
    private const float OffsetY = -0.8f; //between start point in boardEntity and ringSpawner
    private const float ClassicBoardPosY = 0.78f; 
    private const float ContainerPosY = 2.27f; 
    private BoardShape _boardShape;
    private float _cameraSizeCache;
    private float _orthorSize;
    private float _maxSize;
    private bool _isRotateBoard;
    
#if UNITY_EDITOR && TEST_SHAPE
    private void OnEnable()
    {
        _boardShape = customBoardEntity.BoardShape;
        GetMaxSizeAndSetRotate(_boardShape.RotateZ, _boardShape.Size.x, _boardShape.Size.y);
        _cameraSizeCache = Camera.main?.orthographicSize ?? ClassicCameraSize;
        customBoardEntity.transform.position = GetBoardPos();
        ringSpawner.transform.position = GetRingSpawnPosAndSetContainerPos();
        if (Camera.main is null) return;
        _orthorSize = ClassicCameraSize * _maxSize / 3;
        Camera.main.orthographicSize = _orthorSize;
        SettingGateScale();
    }
#else

    private void Awake()
    {
        _boardMask = GetComponent<SpriteMask>();
        _boardRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _boardShape = customBoardEntity.BoardShape;
        if (IsClassicBoard())
        {
            SetupClassicBoard();
            return;
        }
        SetupCustomBoard();
        GetMaxSizeAndSetRotate(_boardShape.RotateZ, _boardShape.Size.x, _boardShape.Size.y);
        _cameraSizeCache = Camera.main?.orthographicSize ?? ClassicCameraSize;
        customBoardEntity.transform.position = GetBoardPos();
        ringSpawner.transform.position = GetRingSpawnPosAndSetContainerPos();
        if (Camera.main is null) return;
        _orthorSize = ClassicCameraSize * _maxSize / 3;
        Camera.main.orthographicSize = _orthorSize;
        SettingGateScale();
    }
#endif
    
    private void GetMaxSizeAndSetRotate(float rotateZ, int xSize,int ySize)
    {
        _isRotateBoard = (rotateZ != 0);
        customBoardEntity.transform.Rotate(new Vector3(0, 0, rotateZ));
        _maxSize = _boardShape.MaxSize;
        var rate = 0.85f;
        if (_maxSize * rate > _boardShape.Height && _maxSize > 6.5f)
        {
            _maxSize *= rate;
        }
    }

    private bool IsClassicBoard()
    {
        if (_boardShape is null) return true;
        return _boardShape.IsClassicBoard();
    }

    private void SetupClassicBoard()
    {
        _boardMask.enabled = true;
        _boardRenderer.enabled = true;
    }

    private void SetupCustomBoard()
    {
        _boardMask.enabled = false;
        _boardRenderer.enabled = false;
    }
    
    private Vector3 GetBoardPos()
    {
        const float boardOffsetY = 0.26f;
        var size = _maxSize;
        if (_isRotateBoard) size -= 0.65f;
        var offset = boardOffsetY * (size - 3);
        return new Vector3(0, ClassicBoardPosY + offset + _boardShape.DeltaRotatePos, 0);
    }

    private void SettingGateScale()
    {
        if (_maxSize <= 5.5f) return;
        var rate = _orthorSize / _cameraSizeCache;
        var scale = (rate - 1) * 0.35f + 1;
        gate.localScale = new Vector3(scale, scale, scale);
    }

    private void SetContainerPos(float height)
    {
        if (Math.Abs(_maxSize - 3) < 0.1f)
        {
            comboContainer.position = new Vector3(0, ContainerPosY, 0);
            return;
        }

        const float multi = 0.7f;
        var offset = height * customBoardEntity.TileSize.y * 0.5f * multi;
        comboContainer.position = new Vector3(0, offset + customBoardEntity.transform.position.y, 0);
    }
    
    private Vector3 GetRingSpawnPosAndSetContainerPos()
    {
        var height = _boardShape.Height;
        SetContainerPos(height);
        
        var yStart = (-_maxSize * customBoardEntity.TileSize.y * 0.5f + customBoardEntity.transform.position.y);
        float multi = _isRotateBoard ? -0.22f : -0.26f;
        var offset = OffsetY + (_maxSize - 3) * multi;
        return new Vector3(0, yStart + offset - _boardShape.DeltaRotatePos, 0);
    }

    private void OnDisable()
    {
        if (Camera.main is null) return;
        Camera.main.orthographicSize = _cameraSizeCache;
    }
    
}