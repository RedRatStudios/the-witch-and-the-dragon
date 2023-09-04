using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessage : MonoBehaviour
{
    [SerializeField] private GameObject emoteTemplate;
    private TextMeshProUGUI chattername;

    private void Start()
    {
        chattername = GetComponentInChildren<TextMeshProUGUI>();
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
}
