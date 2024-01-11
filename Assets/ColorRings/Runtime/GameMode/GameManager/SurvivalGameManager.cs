using System;
using System.Collections;
using Jackie.Soft;
using UnityEngine;

public class SurvivalGameManager : GameManager
{
    [Range(3, 8)] [SerializeField] private int minRingsToMatch = 3; 
    private ILimitStrategy _limitStrategy;

    public override void SetUp(Level level, Skin skin)
    {
        base.SetUp(level, skin);
        StartCoroutine(IEStartChallenge());
    }

    private IEnumerator IEStartChallenge()
    {
        yield return new WaitForSeconds(0.5f);
        Message.Use<Type>().Event(typeof(SurvivorChallenge)).Execute<SurvivorChallenge>(e =>
        {
            e.OnOutOfTime += OnOutOfTime;
        });
    }

    private void OnOutOfTime()
    {
        StartCoroutine(IECancelInsert());
        OnLose();
    }
}