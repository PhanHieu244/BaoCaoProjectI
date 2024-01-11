using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RewardGiftEffectEntity : MonoBehaviour
{
    [SerializeField] private Text infoText;
    [SerializeReference, SubclassSelector] private IGiftEffectTarget _giftEffectTarget;
    public void SetInfo()
    {
        
    }

    public IEnumerator PlayEffect(Vector3 start)
    {
        yield break;
    }
    
    public IEnumerator PlayEffect(Vector3 start, Vector3 end)
    {
        yield break;
    }
}