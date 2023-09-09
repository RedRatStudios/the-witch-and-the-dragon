using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;
    [SerializeField] private GameObject template;
    [SerializeField] private Transform container;
    private static UnityEngine.Object[] shopIconImages;
    
    private void Awake()
    {
        shopIconImages =  Resources.LoadAll("Shop", typeof(Sprite));
        Instance = this;
    }

    private void Start()
    {
        template.SetActive(false);

        // TODO: is this testing code ? idk
        foreach (Upgrade upgrade in Upgrade.AllUpgrades)
        {
            var newButton = Instantiate(template, container);
            var upgradebutton = newButton.GetComponent<BuyUpgradeButton>();
            upgradebutton.upgradeTitle = upgrade.Name;
            Debug.Log("Go fuck yourself.");
            Debug.Log(upgrade.Name);
            newButton.GetComponent<Image>().sprite = Array.Find(shopIconImages, e => e.name == upgrade.Name) as Sprite;
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
