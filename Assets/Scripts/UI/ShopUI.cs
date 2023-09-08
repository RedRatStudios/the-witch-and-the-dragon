using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;
    [SerializeField] private GameObject template;
    [SerializeField] private Transform container;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        template.SetActive(false);

        // TODO: is this testing code ? idk
        foreach (Upgrade upgrade in Upgrade.AllUpgrades)
        {
            var newButton = Instantiate(template, container);
            newButton.GetComponent<BuyUpgradeButton>().upgradeTitle = upgrade.Name;
            newButton.SetActive(true);
        }
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        Debug.Log("Showing");
        gameObject.SetActive(true);
        foreach (var button in BuyUpgradeButton.AllButtons)
            button.UpdateVisuals();
    }

    public void Toggle()
    {
        if (gameObject.activeSelf) Hide();
        else Show();
    }
}
