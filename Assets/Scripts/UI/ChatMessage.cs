using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessage : MonoBehaviour
{
    [SerializeField] private GameObject emoteTemplate;
    [SerializeField] private GameObject chatterNameTemplate;

    private void Start()
    {
        chatterNameTemplate.SetActive(false);
        emoteTemplate.SetActive(false);
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
        var newChatterName = Instantiate(chatterNameTemplate, transform).GetComponent<TextMeshProUGUI>();
        newChatterName.color = new Color32(Red, Green, Blue, 255);
        newChatterName.text = $"{name}: ";
    }
} 