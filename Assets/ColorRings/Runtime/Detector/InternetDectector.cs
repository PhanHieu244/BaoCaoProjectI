using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InternetDectector : PersistentSingleton<InternetDectector>
{
    [SerializeField] private Canvas warningPopUp;
    [SerializeField] private Button hideBut;
    private const float TimeToCheck = GameConst.TimeCheckInternet;
    private const float TimeShowPopUpAgain = 1f;
    private bool _isBlock = false;
    private float _timeCount = 0;
    
    private bool InternetAvailable => Application.internetReachability != NetworkReachability.NotReachable;

    protected override void Awake()
    {
        base.Awake();
        hideBut.onClick.AddListener(HidePopUp);
    }

    private void OnEnable()
    {
        warningPopUp.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_isBlock)
        {
            if (InternetAvailable) HidePopUp();
            return;
        }
        
        if (_timeCount <= 0)
        {
            if (!InternetAvailable)
            {
                warningPopUp.gameObject.SetActive(true);
                _isBlock = true;
                return;
            }
            _timeCount = TimeToCheck;
        }
        _timeCount -= Time.deltaTime;
    }

    private void HidePopUp()
    {
        warningPopUp.gameObject.SetActive(false);
        _isBlock = false;
        _timeCount = TimeShowPopUpAgain;
    }

}