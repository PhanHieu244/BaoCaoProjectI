using System;
using System.Collections.Generic;
using System.Linq;
using Jackie.Soft;
using UnityEngine;

public class ChallengeManager : MonoBehaviour, Message.ICallback
{
    [SerializeField] private ColorChallenge colorChallengePrefab;
    [SerializeField] private SurvivorChallenge survivorChallengePrefab;
    private bool _challengeIsComplete; 
    private ChallangeType challengeType;
    private List<IChallenge> _challenges = new();

    public bool ChallengeIsComplete => _challengeIsComplete;
    
    public void Submit()
    {
        if (_challenges.Any(t => !t.IsComplete())) return;
        if (challengeType == ChallangeType.Endless) return;
        _challengeIsComplete = true;
        GameManager.Instance.CheckTarget();
    }  

    private void OnEnable()
    {
        Message.Use<Type>().With(this).Sub(typeof(ChallengeManager));
    }

    private void OnDisable()
    {
        Message.Use<Type>().With(this).UnSub(typeof(ChallengeManager));
    }

    public List<ColorChallenge> GetColorChallenge()
    {
        var colorChallenges = new List<ColorChallenge>();
        foreach (var challenge in _challenges)
        {
            if(challenge is not ColorChallenge colorChallenge) continue;
            colorChallenges.Add(colorChallenge);
        }

        return colorChallenges;
    }
    
    public void SetUp(IEnumerable<IChallengeData> challengesData) 
    {
        var temp = 0;
        foreach (var data in challengesData)
        {
            temp++;
            var challenge = Create(data, transform);
            if (challenge != null)
                _challenges.Add(challenge);
        }
        if (temp == 0)
        {
            challengeType = ChallangeType.Endless;
        }
    }

    private IChallenge Create(IChallengeData data, Transform challengeParent)
    {
        switch (data)
        {
            case ColorChallenge.ColorData colorData:
            {
                var challenge = Instantiate(colorChallengePrefab, challengeParent);
                challenge.Set(colorData);
                return challenge;
            }
            case SurvivorChallenge.SurvivalData survivalData:
            {
                var challenge = Instantiate(survivorChallengePrefab, challengeParent);
                challenge.Set(survivalData);
                return challenge;
            }
            default:
                return null;
        }
    }

    public ChallangeType GetChallangeType()
    {
        return challengeType;
    }
}

public interface IChallengeData
{
    
}

public enum ChallangeType
{
    Adventure,
    Endless
}
