using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : _MenuButton
{
    void Start()
    {
        button.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
