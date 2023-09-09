using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OptionsUI : MonoBehaviour, IAmVeryLazy
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

public interface IAmVeryLazy
{
    public void Hide();
    public void Show();
}