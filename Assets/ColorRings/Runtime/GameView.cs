using UnityEngine;

[ExecuteInEditMode]
public class GameView : MonoBehaviour
{
    [SerializeField] private Vector2Int viewPort = new(1080, 1920);
    [SerializeField] private float cameraSize = 3.4f;
    private Camera _camera;
    private float _defaultRatio;

    private void Awake()
    {
        _camera = Camera.main;
        _defaultRatio = viewPort.x * 1f / viewPort.y;
        ResizeCamera();
    }

    private void ResizeCamera()
    {
        var ratio = _camera.aspect;
        if (ratio < _defaultRatio)
        {
            _camera.orthographicSize = cameraSize * _defaultRatio / ratio;
        }
        else
        {
            _camera.orthographicSize = cameraSize;
        }
    }
#if UNITY_EDITOR
    private void Update() => ResizeCamera();
#endif
}
