using TMPro;
using UnityEngine;

public class MonocoinUI : MonoBehaviour
{
    TextMeshProUGUI textMesh;

    private void Start()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
        UpdateMonocoinUI();

        MonocoinManager.OnMonocoinsChanged += () => UpdateMonocoinUI();
    }

    private void UpdateMonocoinUI()
    {
        long coins = MonocoinManager.Instance.Monocoins;
        string newValue;

        if (coins >= 1000000)
            newValue = $"{coins / 1000000}.{coins % 1000000 / 100000}M";
        else if (coins >= 10000)
            newValue = $"{coins / 1000}.{coins % 1000 / 100}k";
        else newValue = $"{coins}";

        textMesh.text = $"{newValue}";
    }
}