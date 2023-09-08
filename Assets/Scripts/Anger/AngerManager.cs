using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngerManager : MonoBehaviour
{
    public static AngerManager Instance { get; private set; }
    public float Anger;
    public float AngerMultiplier { get; private set; } = 1f;

    public event Action<float> OnAngerChange;
    public event Action OnMaxAnger;
    public event Action OnWithinFirstAngerThreshold;
    public event Action OnWithinSecondAngerThreshold;
    public event Action OnWithinThirdAngerThershold;

    [SerializeField] private float angerFirstThreshold = 0.33f;
    [SerializeField] private float angerSecondThreshold = 0.66f;
    [SerializeField] private float angerLimit = 1f;

    private void Awake()
    {
        Instance = this;
        Anger = 0;
    }

    private void Update()
    {
        // PROTOTYPE
        IncreaseAnger(.003f);
    }

    public void IncreaseAnger(float amount = 0.01f)
    {
        if (Anger == angerLimit) return;

        float lastValue = Anger;

        Anger += amount * AngerMultiplier;
        if (Anger > angerLimit) Anger = angerLimit;

        OnAngerChange?.Invoke(Anger);

        CheckThresholds(lastValue, Anger);
    }


    public void DecreaseAnger(float amount = 0.01f)
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
        if (lastValue < angerFirstThreshold && angerFirstThreshold < anger)
        {
            OnWithinSecondAngerThreshold?.Invoke();
            return;
        }

        if (lastValue < angerSecondThreshold && angerSecondThreshold < anger)
        {
            OnWithinThirdAngerThershold?.Invoke();
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
