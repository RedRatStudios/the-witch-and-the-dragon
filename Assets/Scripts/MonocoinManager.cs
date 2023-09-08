using System;
using System.Collections.Generic;
using UnityEngine;

public class MonocoinManager : MonoBehaviour
{
    public static MonocoinManager Instance;
    public static event Action OnMonocoinsChanged;
    public static event Action OnNotEnoughMonocoins;

    [SerializeField] public float globalMonocoinPowerscale = 5f;
    [SerializeField] public float globalMonocoinMultiplier = 5f;
    [SerializeField] public float globalEchoMultiplier = 5f;

    [SerializeField] float multiplierFloor = 2;
    [SerializeField] float multiplierCeiling = 3;

    private long monocoinsAvailable = 0;
    private float tickTimer = 0f;
    private float tickTimerCeil = 0.2f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AlchemyManager.OnIngredientsCombinedResultingMessage += message =>
            AwardMonocoinsForCombine(message);
    }

    private void Update()
    {
        AddCoinsFromEcho();

        // TODO: remove testing code
        if (Input.GetKeyDown(KeyCode.A)) AddMonocoins(10);
        // TODO: GUI button
        if (Input.GetKeyDown(KeyCode.Escape)) ShopUI.Instance.Toggle();
    }

    private void AddCoinsFromEcho()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer > tickTimerCeil)
        {
            tickTimer = 0f;

            int coinsAdded = (int)(Upgrade.Echo.TimesBought * globalEchoMultiplier * AngerManager.Instance.Anger);
            AddMonocoins(coinsAdded, true);
        }
    }

    private void AwardMonocoinsForCombine(Message message)
    {
        float randomMult = UnityEngine.Random.Range(multiplierFloor, multiplierCeiling);

        // Multiply by 10 so that square actually scales it up instead of down
        // Random multiplier on top of squared number
        float amount = Mathf.Pow(message.funny * 10, 2) * randomMult;
        AddMonocoins(amount);
    }

    public void TryBuyWithMonocoins(int amount, out bool success)
    {
        success = true;

        if (amount > monocoinsAvailable)
        {
            OnNotEnoughMonocoins?.Invoke();
            success = false;
            return;
        }

        monocoinsAvailable -= amount;
        OnMonocoinsChanged?.Invoke();
    }

    public void AddMonocoins(float amount, bool bypassExponent = false)
    {
        float newAmount;

        if (bypassExponent)
        {
            newAmount = amount;
            goto ApplyMonocoins;
        }

        float markiplier = PlayerStats.Instance.marbleMultiplier
                         * globalMonocoinPowerscale;

        newAmount = amount
              * globalMonocoinMultiplier
              * PlayerStats.Instance.monocoinMultplier;

        Debug.Log("====================MONOCOINS====================");
        Debug.Log($"Amount: {newAmount}, from {amount} by global {globalMonocoinMultiplier} and {PlayerStats.Instance.monocoinMultplier} \n"
                + $"Exponent: {markiplier}, from global {globalMonocoinPowerscale} by {PlayerStats.Instance.marbleMultiplier} \n"
                + $"For total of: {(long)Math.Pow(newAmount, markiplier)}");

        newAmount = Mathf.Pow(newAmount, markiplier);

    ApplyMonocoins:
        newAmount *= PlayerStats.Instance.monocoinSmallMultiplier;

        monocoinsAvailable += (long)newAmount;
        OnMonocoinsChanged?.Invoke();
    }

    public long Monocoins => monocoinsAvailable;
}