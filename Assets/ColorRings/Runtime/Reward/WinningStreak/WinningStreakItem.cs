using System.Collections;
using Coffee.UIExtensions;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WinningStreakItem : MonoBehaviour
{
    [SerializeField] private Image itemImg;
    [SerializeField] private UIParticle trail;
    private PowerUpType _powerUpType;
    private const float DesScaleTime = 0.2f;
    private const float InsScaleTime = 0.1f;
    private const float MoveTime = 0.85f;
    private const float TimeBeforeFade = 0.12f;
    private const float TimeToWorld = 0.35f;
    private Transform _newParent;
    
    public void Setup(PowerUpType powerUpType, Sprite itemImage)
    {
        itemImg.sprite = itemImage;
        _powerUpType = powerUpType;
    }

    public IEnumerator MoveToItem(Transform newParent)
    {
        trail.Play();
        _newParent = newParent;
        var startPos = transform.position;
        var targetPos = UIInGame.Instance.GetItemPos(_powerUpType);
        var pathPos = new Vector3( startPos.x + (targetPos.x - startPos.x) * 0.4f, startPos.y + (startPos.y - targetPos.y) * 0.5f, startPos.z);
        Vector3[] pathPoints = new Vector3[] { startPos, pathPos, targetPos};

        transform.DOScale(new Vector3(1.2f, 0.8f, 1f), DesScaleTime).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(DesScaleTime);
        transform.DOScale(new Vector3(1, 1f, 1f), InsScaleTime).SetEase(Ease.OutBack);
        
        /*transform.DOPath(pathPoints, MoveTime).SetEase(Ease.InCubic).OnComplete(() =>
        {
            UIManager.onWinningStreakReward.Invoke(_powerUpType);
        });*/
        StartCoroutine(IEMoveBezier(startPos, pathPos, targetPos, MoveTime));
        
    }
    private IEnumerator IEMoveBezier(Vector3 p0, Vector3 p1, Vector3 p2, float duration)
    {
        float t = 0;
        var delta = Time.deltaTime / duration;
        bool doFade = false, changeParent = false;
        while (t <= 1)
        {
            var r = 1 - t;
            // B(t) = (1-t)^2 * P0 + 2 * (1-t) * t * P1 + t^2 * P2
            transform.position = r * r * p0 + 2 * r * t * p1 + t * t * p2;
            t += delta;
            switch (t)
            {
                case > 0.92f when !doFade:
                    transform.DOScale(new Vector3(0.3f, 0.3f, 1f), TimeBeforeFade * 1.5f).SetEase(Ease.OutBack);
                    itemImg.DOFade(0.3f, TimeBeforeFade).SetEase(Ease.OutQuad);
                    doFade = true;
                    yield return null;
                    continue;
                case > 0.3f when !changeParent:
                    changeParent = true;
                    transform.SetParent(_newParent);
                    yield return null;
                    continue;
                default:
                    yield return null;
                    break;
            }
        }
        transform.position = p2;
        UIManager.onWinningStreakReward.Invoke(_powerUpType);
        trail.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}