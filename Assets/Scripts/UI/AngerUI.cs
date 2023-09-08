using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AngerUI : MonoBehaviour
{
    [SerializeField] private Image meterImage;
    [SerializeField] private TextMeshProUGUI titleText;

    private void Start()
    {
        meterImage.fillAmount = 0f;
        titleText.text = "Joe's Anger";

        AngerManager.Instance.OnAngerChange += UpdateVisual;

        // TODO: remove testing code
        AngerManager.Instance.OnWithinFirstAngerThreshold += () => titleText.text = "Joe's Anger";
        AngerManager.Instance.OnWithinSecondAngerThreshold += () => titleText.text = "One third full";
        AngerManager.Instance.OnWithinThirdAngerThershold += () => titleText.text = "Two thirds full";
        AngerManager.Instance.OnMaxAnger += () => BanAnimation();
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
