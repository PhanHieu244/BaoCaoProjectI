using UnityEngine;

public class GiftTargetDectector : MonoBehaviour
{
    [SerializeField] public Transform coinTarget;
    [SerializeField] public Transform powerUpTarget;

    public static Transform CoinTargetPos { get; private set; }
    public static Transform PowerUpTargetPos { get; private set; }

    private void OnEnable()
    {
        CoinTargetPos = coinTarget;
        PowerUpTargetPos = powerUpTarget;
    }
}