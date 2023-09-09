using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AngerUI : MonoBehaviour
{
    [SerializeField] private Image meterImage;

    private void Start()
    {
        meterImage.fillAmount = 0f;

        AngerManager.OnAngerChange += UpdateVisual;
        AngerManager.OnMaxAnger += () => BanAnimation();
    }

    private void OnDestroy()
    {
        AngerManager.OnAngerChange -= UpdateVisual;
        AngerManager.OnMaxAnger -= () => BanAnimation();

    }
    private void BanAnimation()
    {
        //TODO: Show splash, lock UI
    }

    private void UpdateVisual(float value)
    {
        meterImage.fillAmount = value;
    }
}
