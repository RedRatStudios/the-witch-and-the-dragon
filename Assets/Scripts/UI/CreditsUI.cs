using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreditsUI : MonoBehaviour, IAmVeryLazy
{
    private void Start()
    {
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Toggle()
    {
        if (gameObject.activeSelf) Hide();
        else Show();
    }
}