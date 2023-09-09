using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngerManager : MonoBehaviour
{
    public static AngerManager Instance { get; private set; }
    public float Anger;
    public event Action<float> OnAngerChange;
    public event Action OnMaxAnger;
    public event Action OnWithinFirstAngerThreshold;
    public event Action OnWithinSecondAngerThreshold;
    public event Action OnWithinThirdAngerThershold;

    [SerializeField] private float globalAngerPowerscale = 2;
    [SerializeField] private float globalAngerMultiplier = 10;
    [SerializeField] private float maxAngerAtATime = 0.7f;
    [SerializeField] private float angerDropoff = 0.0012f;
    [SerializeField] private float angerFirstThreshold = 0.33f;
    [SerializeField] private float angerSecondThreshold = 0.66f;
    [SerializeField] private float angerLimit = 1f;

    private float tickTimer = 0f;
    private float tickTimerCeil = 0.2f;

    private void Awake()
    {
        Instance = this;
        Anger = 0;
    }

    private void Start()
    {
        AlchemyManager.OnIngredientsCombinedResultingMessage += message =>
            IncreaseAnger(message.annoying);

        SceneMoodManager.Instance.OnMoodUpdate += mood =>
        {
            Anger = 0f;
            OnAngerChange?.Invoke(Anger);
        };

    }

    private void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer < tickTimerCeil) return;

        tickTimer = 0f;
        DecreaseAnger(angerDropoff);
    }

    public void IncreaseAnger(float amount)
    {
        if (Anger == angerLimit) return;
        float lastValue = Anger;
        float newAmount;

        newAmount = amount
                  * globalAngerMultiplier;

        float markiplier = PlayerStats.Instance.AngerMultiplier
                         * globalAngerPowerscale;

        // I don't know what the fuck exactly is going on here, all I know is that it works.
        newAmount += Mathf.Pow(newAmount, markiplier) / 100;

        Debug.Log("====================ANGER====================");
        Debug.Log($"Amount: {newAmount}, from {amount} by global {globalAngerMultiplier} and {PlayerStats.Instance.monocoinMultplier} \n"
                + $"Exponent: {markiplier}, from global {globalAngerPowerscale} by {PlayerStats.Instance.marbleMultiplier} \n"
                + $"For total of: {(long)Math.Pow(newAmount, markiplier)}");

        // Apply a max change threshold so that it doesn't feel unfair
        if (newAmount > lastValue + maxAngerAtATime)
            newAmount = lastValue + maxAngerAtATime;

        Anger += newAmount;

        // stop at 100%
        if (Anger > angerLimit) Anger = angerLimit;

        OnAngerChange?.Invoke(Anger);
        CheckThresholds(lastValue, Anger);
    }


    public void DecreaseAnger(float amount)
    {
        if (Anger == 0) return;

        float lastValue = Anger;

        Anger -= amount;
        if (Anger < 0) Anger = 0;

        OnAngerChange?.Invoke(Anger);

        CheckThresholds(lastValue, Anger);
    }

    private void CheckThresholds(float lastValue, float anger)
    {
        if (anger == angerLimit)
        {
            OnMaxAnger?.Invoke();
            return;
        }

        if (anger == 0)
        {
            OnWithinFirstAngerThreshold?.Invoke();
            return;
        }

        // This is ugly as shit.
        if (lastValue < angerSecondThreshold && angerSecondThreshold < anger)
        {
            OnWithinThirdAngerThershold?.Invoke();
            return;
        }

        if (lastValue < angerFirstThreshold && angerFirstThreshold < anger)
        {
            OnWithinSecondAngerThreshold?.Invoke();
            return;
        }

        if (lastValue > angerFirstThreshold && angerFirstThreshold > anger)
        {
            OnWithinFirstAngerThreshold?.Invoke();
            return;
        }

        if (lastValue > angerSecondThreshold && angerSecondThreshold > anger)
        {
            OnWithinSecondAngerThreshold?.Invoke();
            return;
        }

        if (lastValue >= angerLimit && anger < angerLimit)
        {
            OnWithinThirdAngerThershold?.Invoke();
            return;
        }
    }

}
