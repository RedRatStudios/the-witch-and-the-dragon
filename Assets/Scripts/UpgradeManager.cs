using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    private float rogueModTimerCeiling = 30f;

    public static event Action OnRogueModSuccess;
    public static event Action OnRogueModFail;
    public static event Action OnAnyUpgradeBought;


    private void Awake()
    {
        Instance = this;


        // ╭────────────────────────────────────────────────────────────────────╮
        // │ Initializing upgrades                                              │
        // ╰────────────────────────────────────────────────────────────────────╯

        string[] echoPerks = {  "Joe's anger will rise quicker, but you passively gain Monocoins.",
                                "You gain more coins the higher the meter is.",
                                "Stackable"
                                };
        Upgrade.Echo = new(1000, "Echo",
                    "Increase Joe's microphone echo slightly", echoPerks);
        Upgrade.Echo.stackable = true;
        Upgrade.Echo.OnBought += () => OnBoughtEcho();

        string[] plasmidsPerks = { "Instantly reduce Joe's anger.", "Very slightly decrease cost creep.", "Stackable" };
        Upgrade.Plasmids = new(5000, "Plasmids",
                    "Buy some plasmids for Joe to calm him down.", plasmidsPerks);
        Upgrade.Plasmids.stackable = true;
        Upgrade.Plasmids.OnBought += () => OnBoughtPlasmids();

        string[] rerollPerks = { "Reroll available ingredients", "Stackable" };
        Upgrade.Reroll = new(5000, "Call up QT-Chan",
                    "Make your ingredients quantum briefly", rerollPerks);
        Upgrade.Reroll.stackable = true;
        Upgrade.Reroll.OnBought += () => OnBoughtReroll();


        string[] highlightPerks = {  "Next message will be much funnier and much more annoying.",
                                     "1% chance of getting banned instantly if your message is even slightly annoying.",
                                     "Stackable" };
        Upgrade.Highlight = new(10000, "Highlight Message",
                    "Highlight your next message to increase its effect", highlightPerks);
        Upgrade.Highlight.stackable = true;
        Upgrade.Highlight.OnBought += () => OnBoughtHighlight();

        string[] undercourtPerks = { "Survive getting banned once" };
        Upgrade.Undercourt = new(100000, "Undercourt's Favor",
                    "Abuse the Judge's literary inadequacy to gain an edge.", undercourtPerks);
        Upgrade.Undercourt.OnBought += () => OnBoughtUndercourt();

        string[] tier1Perks = { "Reduces message cooking time in half", "Unlocks Tier 2 Sub" };
        Upgrade.Sub1 = new(10000, "Tier 1 Sub",
                    "Send 70% of these Monocoins to Jeff Bezos", tier1Perks);
        Upgrade.Sub1.OnBought += () => OnBoughtSub1();

        string[] tier2Perks = { "Reduce reroll cost in half", "1.5x Monocoin payout", "Unlocks Tier 3 Sub" };
        Upgrade.Sub2 = new(50000, "Tier 2 Sub",
                    "There are people who boughtthis just to use jphHella.", tier2Perks);
        Upgrade.Sub2.locked = true;
        Upgrade.Sub2.OnBought += () => OnBoughtSub2();

        string[] tier3Perks = { "Messages cook insantly", "3x Monocoin payout" };
        Upgrade.Sub3 = new(200000, "Tier 3 Sub",
                    "You really miss the Patreon, don't you?", tier3Perks);
        Upgrade.Sub3.locked = true;
        Upgrade.Sub3.OnBought += () => OnBoughtSub3();

        string[] roguePerks = { "70% chance to double your investment after 30 seconds",
                                "Stackable, price increases forever." };
        Upgrade.RogueMod = new(300000, "Rogue Mod",
                    "Strike a deal with a mod to scam everyone else out of their coins.", roguePerks);
        Upgrade.RogueMod.stackable = true;
        Upgrade.RogueMod.TimesBought = PlayerPrefs.GetInt("rogueModsBought", 0);
        Upgrade.RogueMod.OnBought += () => OnBoughtRogue();

        string[] weebPerks = { "Achieve inner peace" };
        Upgrade.Absolution = new(1000000, "Weeb Absolution",
                    "You finally don't have to do this anymore.", weebPerks);
        Upgrade.Absolution.OnBought += () => OnBoughtAbsolution();

        string[] marblePerks = { "Your meassages are 1% funnier, forever.",
                                 "Stackable, price stays the same." };
        Upgrade.Marble = new(1000000, "Buy a Marble",
                    "The worse option.", marblePerks);
        Upgrade.Marble.stackable = true;
        Upgrade.Marble.TimesBought = PlayerPrefs.GetInt("marblesBought", 0);
        Upgrade.Marble.OnBought += () => OnBoughtMarble();
    }

    private void Start()
    {
        Upgrade.Marble.TimesBought = PlayerPrefs.GetInt("marblesBought", 0);
        Upgrade.RogueMod.TimesBought = PlayerPrefs.GetInt("rogueModsBought", 0);

    }


    // ╭────────────────────────────────────────────────────────────────────╮
    // │ Upgrade functionality                                              │
    // ╰────────────────────────────────────────────────────────────────────╯

    private void OnBoughtMarble()
    {
        PlayerPrefs.SetInt("marblesBought", Upgrade.Marble.TimesBought);
        PlayerStats.Instance.marbleMultiplier *= 1.01f;
        PlayerPrefs.SetFloat("marbleMultiplier", PlayerStats.Instance.marbleMultiplier);
        PlayerPrefs.Save();
    }

    private void OnBoughtAbsolution()
    {

        // play win animation
        throw new NotImplementedException();
    }

    private void OnBoughtRogue()
    {
        PlayerPrefs.SetInt("rogueModsBought", Upgrade.RogueMod.TimesBought);
        PlayerPrefs.Save();

        StartCoroutine(RogueModTimer(Upgrade.RogueMod.ActualCost));
        Upgrade.RogueMod.ActualCost = (int)CalcStackablePrice(Upgrade.RogueMod.BaseCost, Upgrade.RogueMod.TimesBought, 1.2f);
    }

    private IEnumerator RogueModTimer(int price)
    {
        yield return new WaitForSeconds(rogueModTimerCeiling);

        float chance = UnityEngine.Random.Range(0f, 1f);
        if (chance < .7)
        {
            MonocoinManager.Instance.AddMonocoins(price * 2);
            OnRogueModSuccess?.Invoke();
        }
        else
            OnRogueModFail?.Invoke();
    }

    private void OnBoughtSub3()
    {
        PlayerStats.Instance.cookingTimeMultiplier *= 10f;
        PlayerStats.Instance.monocoinMultplier *= 3f;
        Upgrade.Sub3.locked = true;
    }

    private void OnBoughtSub2()
    {
        PlayerStats.Instance.monocoinMultplier *= 1.5f;
        Upgrade.Sub3.locked = false;
        Upgrade.Sub2.locked = true;
    }

    private void OnBoughtSub1()
    {
        PlayerStats.Instance.cookingTimeMultiplier *= 2f;
        Upgrade.Sub2.locked = false;
        Upgrade.Sub1.locked = true;
    }

    private void OnBoughtUndercourt()
    {
        PlayerStats.Instance.surviveBan = true;
        Upgrade.Undercourt.locked = true;
    }

    private void OnBoughtHighlight()
    {
        PlayerStats.Instance.highlithedMessage = true;
        Upgrade.Highlight.ActualCost = (int)CalcStackablePrice(Upgrade.Highlight.BaseCost, PlayerStats.Instance.highlightsBought);

        Upgrade.Highlight.locked = true;
    }

    private void OnBoughtReroll()
    {
        Upgrade.Reroll.ActualCost = (int)CalcStackablePrice(Upgrade.Reroll.BaseCost, PlayerStats.Instance.rerollsBought);

        // Reroll upgrades
        throw new NotImplementedException();
    }

    private void OnBoughtPlasmids()
    {
        Upgrade.Plasmids.ActualCost = (int)CalcStackablePrice(Upgrade.Plasmids.BaseCost, PlayerStats.Instance.plasmidsBought);
        PlayerStats.Instance.costCreep -= 0.002f;
        PlayerPrefs.SetFloat("costCreep", PlayerStats.Instance.costCreep);
        PlayerPrefs.Save();

        // Reduce anger
        throw new NotImplementedException();
    }

    private void OnBoughtEcho()
    {
        PlayerStats.Instance.echoesBought += 1;
        Upgrade.Echo.ActualCost = (int)CalcStackablePrice(Upgrade.Echo.BaseCost, PlayerStats.Instance.echoesBought);
    }

    private float CalcStackablePrice(float baseCost, int stacks, float multplier = 0f)
    {
        if (multplier == 0)
            multplier = PlayerStats.Instance.costCreep;

        float newPrice = baseCost;
        for (int i = 1; i <= stacks; i++)
            newPrice *= multplier;

        return newPrice;
    }

    // ╭────────────────────────────────────────────────────────────────────╮
    // │ Public functions                                                   │
    // ╰────────────────────────────────────────────────────────────────────╯

    public static void BuyUpgrade(Upgrade upgrade, out bool success)
    {
        Debug.Log($"trying to buy {upgrade.Name}");

        success = true;
        MonocoinManager.Instance.TryBuyWithMonocoins(upgrade.ActualCost, out bool boughtSuccessfully);

        if (!boughtSuccessfully || upgrade.locked)
        {
            success = false;

            Debug.Log($"fail");
            return;
        }

        upgrade.Buy();
        Debug.Log($"success");
        OnAnyUpgradeBought?.Invoke();
    }
}

public class Upgrade
{
    public string Name;
    public string Description;
    public string[] PerkDescriptions;

    public int BaseCost;
    public int ActualCost;
    public int TimesBought;

    public bool locked = false;
    public bool stackable = false;

    public event Action OnBought;

    public Upgrade(int cost, string name, string description, string[] perks)
    {
        ActualCost = BaseCost = cost;
        Name = name;
        Description = description;
        PerkDescriptions = perks;

        AllUpgrades.Add(this);
    }

    public void Buy()
    {
        if (stackable) TimesBought++;
        OnBought?.Invoke();
    }

    public static Upgrade Echo;
    public static Upgrade Plasmids;
    public static Upgrade Reroll;
    public static Upgrade Highlight;
    public static Upgrade Sub1;
    public static Upgrade Sub2;
    public static Upgrade Sub3;
    public static Upgrade Undercourt;
    public static Upgrade RogueMod;
    public static Upgrade Absolution;
    public static Upgrade Marble;

    public static List<Upgrade> AllUpgrades = new();

}