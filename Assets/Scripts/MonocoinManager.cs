using System;
using System.Collections.Generic;
using UnityEngine;

public class MonocoinManager : MonoBehaviour
{
    public static MonocoinManager Instance;
    public static event Action OnMonocoinsChanged;
    public static event Action OnNotEnoughMonocoins;

    [SerializeField] int multiplierFloor = 3;
    [SerializeField] int multiplierCeiling = 5;

    private long monocoinsAvailable = 0;
    private float tickTimer = 0f;
    private float tickTimerCeil = 0.2f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AlchemyManager.OnIngredientsCombined += result => AwardMonocoinsForCombine(result);

    }

    private void Update()
    {
        AddCoinsFromEcho();

        // TODO: remove testing code
        if (Input.GetKeyDown(KeyCode.A)) AddMonocoins(monocoinsAvailable += 1000);
        // TODO: GUI button
        if (Input.GetKeyDown(KeyCode.Escape)) ShopUI.Instance.Toggle();
    }

    private void AddCoinsFromEcho()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer > tickTimerCeil)
        {
            tickTimer = 0f;
            // TODO: multiply by AngerManager.Anger
            int coinsAdded = PlayerStats.Instance.echoesBought * 5;
            AddMonocoins(coinsAdded);
        }
    }

    private void AwardMonocoinsForCombine(Dictionary<string, string> result)
    {
        float randomMult = UnityEngine.Random.Range(multiplierFloor, multiplierCeiling);
        float amount = (float)Convert.ToDouble(result["funny"]) * randomMult * 100;
        AddMonocoins((long)amount);
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

    public void AddMonocoins(long value)
    {
        float markiplier = PlayerStats.Instance.marbleMultiplier +
                           PlayerStats.Instance.monocoinMultplier;
        monocoinsAvailable += (long)(value * markiplier);
        OnMonocoinsChanged?.Invoke();
    }

    public long Monocoins => monocoinsAvailable;
}