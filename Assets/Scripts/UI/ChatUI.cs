using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatUI : MonoBehaviour
{
    public static ChatUI Instance;
    public event Action OnBurstStart;
    public event Action OnBurstEnd;

    [SerializeField] private GameObject messageContainer;
    [SerializeField] private GameObject messageTemplate;

    // TODO: separate into several arrays in several folders for type tracking
    // Funny, Bad, Random
    private UnityEngine.Object[] allEmoteSpritesArray;

    // TODO: store chatter names and their colors in a CSV

    // Burst - temporarily set lower maximum value for message spawn timer
    private bool burstActive;
    private float burstTimer;
    private float burstTimeout = 3f;

    private float newMessageTimer;
    private float newMessageTimeout;
    [SerializeField] private float timerFloor = .1f;
    [SerializeField] private float timerCeilingNormal = 1f;
    [SerializeField] private float timerCeilingBurst = .2f;

    [SerializeField] private int maxEmotesInMessage = 4;
    [SerializeField] private float sameEmoteSpamChance = .7f;

    [SerializeField] private int maxMessagesOnScreen = 13;

    private List<GameObject> messageGameObjects;

    private void Awake()
    {
        messageTemplate.SetActive(false);
    }

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
            Instance = this;

        newMessageTimer = 0f;
        burstTimer = 0f;

        messageGameObjects = new();

        allEmoteSpritesArray = Resources.LoadAll("Emotes", typeof(Sprite));
    }

    private void Update()
    {
        UpdateBurstTimer();
        UpdateTimer();
        CullMessagesOutOfBounds();
    }

    private void UpdateTimer()
    {
        newMessageTimer += Time.deltaTime;
        if (newMessageTimer < newMessageTimeout) return;

        newMessageTimer = 0f;

        if (burstActive)
            newMessageTimeout = UnityEngine.Random.Range(timerFloor, timerCeilingBurst);
        else
            newMessageTimeout = UnityEngine.Random.Range(timerFloor, timerCeilingNormal);

        SpawnRandomMessage();
    }

    private void UpdateBurstTimer()
    {
        if (!burstActive) return;
        burstTimer += Time.deltaTime;

        // Timer expired
        if (burstTimer < burstTimeout) return;

        burstActive = false;
        newMessageTimeout = timerCeilingNormal;
        burstTimer = 0;

        OnBurstEnd?.Invoke();
    }

    private void CullMessagesOutOfBounds()
    {
        // TODO: this
    }

    private void SpawnRandomMessage() // TODO: pass type
    {
        GameObject messageObject = Instantiate(messageTemplate, messageContainer.transform);
        messageObject.SetActive(true);
        var chatMessage = messageObject.GetComponent<ChatMessage>();

        // TODO: chatMessage.SetChatterName( some name from list, color in RGB )

        int emotesInThisMessage = UnityEngine.Random.Range(1, maxEmotesInMessage);
        bool sameEmoteSpam = UnityEngine.Random.Range(0f, 1f) > sameEmoteSpamChance;

        Sprite emoteSprite = GetRandomEmote();
        for (int i = 0; i < emotesInThisMessage; i++)
        {
            chatMessage.AddEmote(emoteSprite);
            if (!sameEmoteSpam) emoteSprite = GetRandomEmote();
        }

        // Track and cull excess messages
        messageGameObjects.Add(messageObject);
        if (messageGameObjects.Count > maxMessagesOnScreen)
        {
            Destroy(messageGameObjects[0]);
            messageGameObjects.RemoveAt(0);
        }
    }

    private Sprite GetRandomEmote()
    {
        // TODO: introduce parameter for selecting emote type, as in
        // Funny: only return LUL, +2, jphCoggers ...
        // Bad: only return -2, jphChiaki, ResidentSleeper, NotLikeThis
        // Any: return any emote

        int randomIndex = UnityEngine.Random.Range(0, allEmoteSpritesArray.Length);
        return allEmoteSpritesArray[randomIndex] as Sprite;
    }

    // <summary>
    // Call this to make chat go mental.
    // Optionally pass a float for duration.
    // </summary>
    public void ActivateBurst(float burstTimeout = 3f)
    {
        burstActive = true;
        newMessageTimeout = 0.1f; // Don't wait for timer to expire before bursting
        this.burstTimeout = burstTimeout;

        OnBurstStart?.Invoke();
    }
}
