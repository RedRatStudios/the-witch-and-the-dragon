using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuyUpgradeButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    public static List<BuyUpgradeButton> AllButtons = new();

    public static event Action OnButtonPressed;
    public static event Action OnButtonReleased;
    public static event Action OnButtonHovered;

    public string upgradeTitle;

    [SerializeField] protected TextMeshProUGUI titleText;
    [SerializeField] protected TextMeshProUGUI descriptionText;
    [SerializeField] protected TextMeshProUGUI perksText;
    [SerializeField] protected TextMeshProUGUI costText;
    [SerializeField] protected TextMeshProUGUI countText;
    [SerializeField] protected GameObject lockedFadeImage;


    protected Upgrade upgrade;
    protected Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        AllButtons.Add(this);
        foreach (Upgrade upgrade in Upgrade.AllUpgrades)
        {
            if (upgrade.Name == upgradeTitle)
            {
                this.upgrade = upgrade;
                goto InstanceInitialized;
            }
        }
        Debug.LogError($"{this} has an invalid Upgrade title property");

    InstanceInitialized:
        button.onClick.AddListener(() => UpgradeManager.Instance.BuyUpgrade(upgrade, out _));

        UpdateVisuals();
        UpgradeManager.Instance.OnAnyUpgradeBought += () => UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        titleText.text = upgrade.Name;
        descriptionText.text = upgrade.Description;

        string perksTextTemp = "";
        foreach (string desc in upgrade.PerkDescriptions)
            perksTextTemp += $"- {desc} \n";
        perksText.text = perksTextTemp;

        int cost = upgrade.ActualCost;
        if (cost >= 1000000)
            costText.text = $"{cost / 1000000}.{cost % 1000000 / 100000}M";
        else if (cost >= 10000)
            costText.text = $"{cost / 1000}.{cost % 1000 / 100}k";
        else costText.text = $"{cost}";

        if (upgrade.TimesBought <= 0)
            countText.text = "";
        else
            countText.text = upgrade.TimesBought.ToString();

        if (upgrade.locked)
            lockedFadeImage.SetActive(true);
        else
            lockedFadeImage.SetActive(false);
    }


    /* ╭────────────────────────────────────────────────────────────────────╮
       │     Interface implementation                                       │
       ╰────────────────────────────────────────────────────────────────────╯ */

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable || upgrade.locked) return;

        OnButtonPressed?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!button.interactable || upgrade.locked) return;

        OnButtonReleased?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable || upgrade.locked) return;

        OnButtonHovered?.Invoke();
    }
}