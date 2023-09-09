using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowerButton : _MenuButton
{
    [SerializeField] private GameObject menuToShow;

    void Start()
    {
        button.onClick.AddListener(() =>
        {
            parent.Hide();
            menuToShow.GetComponent<IAmVeryLazy>().Show();
        });
    }
}
