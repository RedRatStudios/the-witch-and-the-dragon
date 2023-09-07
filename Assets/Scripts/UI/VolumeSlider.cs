using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer.Groups group;
    private Slider slider;
    private TextMeshProUGUI textMesh;

    private void Awake()
    {
        slider = gameObject.GetComponentInChildren<Slider>();
        textMesh = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        slider.value = SoundManager.Instance.GetGroupVolumeNormalized(group);
        UpdateVisual();

        slider.onValueChanged.AddListener(value =>
        {
            SoundManager.Instance.SetGroupVolume(value, group);
            UpdateVisual();
        });
    }

    private void UpdateVisual()
    {
        float volume = SoundManager.Instance.GetGroupVolumeNormalized(group) * 100;
        textMesh.SetText($"{group}: {Math.Ceiling(volume)}");
    }
}
