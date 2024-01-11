using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ColorRings.Runtime.UI.PopUpInGame
{
    [RequireComponent(typeof(Slider))]
    public class SkinStreakBar : MonoBehaviour
    {
        [SerializeField] protected Slider slider;
        [SerializeField] private float valueSizePoint = 0.32f;
        [FormerlySerializedAs("startValues")] [SerializeField] protected float[] leftPointValues = new float[GameConst.MaxSkinStreak];
        [SerializeField] protected Image[] activePoints;
        [SerializeField] protected Image skinRewardPoint;
        [SerializeField] protected Sprite skinRewardActiveImage;
        protected int currentStreak;
        private float[] _rightPointValues;
        
        
        protected virtual void Awake()
        {
            slider = gameObject.GetComponent<Slider>();
        }

        protected virtual void OnEnable()
        {
            Reset();
            SetUpSkinBar(GameDataManager.RewardSkinStreak);
        }

        [Button]
        protected void Reset()
        {
            slider.value = 0;
            for (int i = 0; i < activePoints.Length; i++)
            {
                activePoints[i].fillAmount = 0;
            }
        }
        
        protected virtual void SetUpSkinBar(int crtStreak)
        {
            _rightPointValues = new float[leftPointValues.Length - 1];
            for (int i = 1; i < leftPointValues.Length - 1; i++)
            {
                _rightPointValues[i] = valueSizePoint + leftPointValues[i];
            }
            currentStreak = crtStreak;
            if (currentStreak == 0)
            {
                slider.value = 0;
                return;
            }
            slider.value = _rightPointValues[currentStreak - 1];
            for (int i = 0; i < currentStreak; i++)
            {
                activePoints[i].fillAmount = 1;
            }

        }
    }
}