using System;
using System.Linq;
using Puzzle.UI;
using UnityEngine;

public class RingHolder : MonoBehaviour
{
    private IEntity[] ringEntities = new IEntity[Board.Depth];
    public static bool IsLock { get; set; }
    private bool _clickFlag = true;
    private Vector3 _offset;
    private bool _isDragging;
    public int orderFront = 2, orderBack = 3;

    public IEntity this[RingSize ringSize]
    {
        get => this[(int)ringSize];
        set => this[(int)ringSize] = value;
    }

    public IEntity this[int size]
    {
        get => ringEntities[size];
        set => ringEntities[size] = value;
    }

    public static implicit operator Color[](RingHolder holder) => holder.ringEntities.Select(entity => entity == null ? Color.NONE : entity.Color).ToArray();

    private void Awake()
    {
        IsLock = false;
    }

    private void OnEnable()
    {
        Hub.OnBeginShow += DisableClick;
        Hub.OnAfterHide += EnableClick;
    }

    private void OnDisable()
    {
        Hub.OnBeginShow -= DisableClick;
        Hub.OnAfterHide -= EnableClick;
    }

    private void DisableClick()
    {
        _clickFlag = false;
        IsLock = true;
    }

    private void EnableClick()
    {
        _clickFlag = true;
        IsLock = false;
    }

    private void OnMouseDown()
    {
        if (!_clickFlag) return;
        if (IsLock) return;
        _isDragging = true;
        
        AudioManager.Instance.PlaySound(EventSound.ClickRing);
        transform.position += new Vector3(0, .5f, 0);
        _offset = transform.position - GetMouseWorldPosition();
    }

    private void OnMouseDrag()
    {
        if (!_clickFlag) return;
        if (IsLock) return;
        if (!_isDragging) return;
        transform.position = GetMouseWorldPosition() + _offset;
    }

    private void OnMouseUp()
    {
        if (!_clickFlag) return;
        if (IsLock) return;
        _isDragging = false;
        var level = GameManager.Instance.Level;
        if (level.tutorialMod != null && !TutorialManager.IsTurotialDone)
        {
            var coordUp = GameManager.Instance.BoardEntity.InputCoordinateAvailable(this.transform.position);
            var targetCoord = level.tutorialMod.GetTargetCoord();
            if (coordUp == targetCoord)
            {
                GameManager.Instance.StartCoroutine(GameManager.Instance.IEInsert(this));
            } else
            {
                GameManager.Instance.StartCoroutine(GameManager.Instance.IECancelInsert(this));
            }
        }
        else
        {
            GameManager.Instance.StartCoroutine(GameManager.Instance.IEInsert(this));
        }
    }
    
    public static Vector3 GetMouseWorldPosition()
    {
#if UNITY_EDITOR
        if (Camera.main == null) throw new NullReferenceException("camera main is null");
#endif
        var mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = -Camera.main.transform.position.z;
        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        return mouseWorldPosition;
    }
}