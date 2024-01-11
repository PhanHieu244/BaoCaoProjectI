using DG.Tweening;
using UnityEngine;

public interface IBoardEffect
{
    void Initialize(BoardBaseEntity boardBaseEntity);
    public void ApplyEffect(Vector2Int coordinate, Color color);
    public void FlashEffect(IMatch matchStrategy, Color color, Direction direction);
}


public abstract class BoardEffect : IBoardEffect
{
    protected BoardBaseEntity _boardBaseEntity;
    
    public virtual void Initialize(BoardBaseEntity boardBaseEntity)
    {
        _boardBaseEntity = boardBaseEntity;
    }

    public virtual void ApplyEffect(Vector2Int coordinate, Color color)
    {
        var visualEffect = PlayEffect(coordinate, "circle", color);
        DOVirtual.DelayedCall(1f, () => PoolManager.Instance["circle"].Release(visualEffect));

        var blinkEffect = PlayEffect(coordinate, "blink", color);
        DOVirtual.DelayedCall(1f, () => PoolManager.Instance["blink"].Release(blinkEffect));
    }
    
    protected ParticleSystem PlayEffect(Vector2Int positionCoord, string effectKey, Color color)
    {
        var visualEffect = PoolManager.Instance[effectKey].Get<ParticleSystem>(_boardBaseEntity.transform);
        var visualEffectMain = visualEffect.main;
        var c = GameManager.Instance.ParticleMaterial(color).color;
        visualEffectMain.startColor = c;
        visualEffect.transform.position = _boardBaseEntity[positionCoord].transform.position;
        visualEffect.Play();
        return visualEffect;
    }
    
    public abstract void FlashEffect(IMatch matchStrategy, Color color, Direction direction);

}