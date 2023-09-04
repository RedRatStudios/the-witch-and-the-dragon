using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessage : MonoBehaviour
{
    [SerializeField] private GameObject emoteTemplate;
    private TextMeshProUGUI chatterName;

    private void Start()
    {
        chatterName = GetComponentInChildren<TextMeshProUGUI>();
        emoteTemplate.SetActive(false);

        // PROTOTYPE
        SetChatterName("DaBulder", 30, 100, 30);
    }

    // <summary>
    // Instantiate a new emote using a provided sprite.
    // It will be positioned automatically via Grid Layout element.
    // </summary>
    public void AddEmote(Sprite emote)
    {
        var newEmote = Instantiate(emoteTemplate, transform);
        newEmote.GetComponent<Image>().sprite = emote;
    }

    public void SetChatterName(string name, byte Red, byte Green, byte Blue)
    {
        chatterName.color = new Color32(Red, Green, Blue, 255);
        chatterName.text = $"{name}: ";
    }
}
