using UnityEngine;

public class LockTile : MonoBehaviour
{
    [SerializeField] private Transform icon;
    private CustomBoardEntity _customBoardEntity;
    private Vector2Int _coordinate;
    
    public void Setup(CustomBoardEntity customBoardEntity, Vector2Int coord)
    {
        _coordinate = coord;
        _customBoardEntity = customBoardEntity;
        icon.rotation = Quaternion.identity;
    }
    
    private void OnMouseDown()
    {
        if (RingHolder.IsLock) return;
        DevLog.Log("Unlock", "Unlock");
        AdsManager.Instance.ShowReward("LOCKTILE_ADS", () =>
        {
            DevLog.Log("Unlock", "Show Ads");
            AudioManager.Instance.PlaySound(EventSound.Click);
            _customBoardEntity.UnlockTile(_coordinate);
            gameObject.SetActive(false);
        });
    }
}
