using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;

public class ChatScript : MonoBehaviour
{
    [SerializeField]
    private Image[] imageFields;
    [SerializeField]
    private float delay = .1f;

    private bool canSendMoreMessages = true;
    private Object[] allEmotes;

    /// <summary>
    /// Loads all emotes from Assests/Resources/Emotes and stores into an Object array;
    /// </summary>
    void Start()
    {
        allEmotes = Resources.LoadAll("Emotes", typeof(Sprite));
    }

    void Update()
    {
        if (canSendMoreMessages)
        {
            StartCoroutine(DisplayNewMessages());
        }
    }

    /// <summary>
    /// Generates a array of X size with emotes.
    /// </summary>
    private Object[] GetRandomEmotes(int amount)
    {
        Object[] selectedEmotes = new Object[amount];

        for (int i = 0; i < amount; i++)
        {
            int index = Random.Range(0, allEmotes.Length);
            selectedEmotes[i] = allEmotes[index];
        }

        return selectedEmotes;
    }

    /// <summary>
    /// Coroutine that repeats after a delay.
    /// This coroutine gets a new set of emotes and offsets the array by the size of this new set,
    /// then, it appends this sets to the array.
    /// </summary>
    private IEnumerator DisplayNewMessages()
    {
        Object[] newMessages = GetRandomEmotes(Random.Range(3, 6));
        int newNumber = newMessages.Length;

        // This is responsible for the chat scrolling up
        for (int i = 0; i < imageFields.Length - newNumber; i++)
        {
            imageFields[i].sprite = imageFields[i + newNumber].sprite;
        }

        // Inserting in the end of the array
        for (int i = 0; i < newMessages.Length; i++)
        {
            int index = imageFields.Length - i - 1;
            imageFields[index].sprite = (Sprite)newMessages[i];
        }

        // Ugly, if you can make this better please help me.
        canSendMoreMessages = false;
        yield return new WaitForSeconds(delay);
        canSendMoreMessages = true;
    }
}
