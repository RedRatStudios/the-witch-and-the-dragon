using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : _GameButton
{

    private void Start()
    {
        button.onClick.AddListener(() => ShopUI.Instance.Toggle());
    }
}
