using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    public static ChatUI Instance;
    public event Action OnBurstStart;
    public event Action OnBurstEnd;

    public enum MessageType { Good, Bad, Any }

    [SerializeField] private GameObject messageContainer;
    [SerializeField] private GameObject messageTemplate;

    private UnityEngine.Object[] goodEmoteSpritesArray;
    private UnityEngine.Object[] badEmoteSpritesArray;
    private UnityEngine.Object[] otherEmoteSpritesArray;
    private UnityEngine.Object[] allEmoteSpritesArray;

    private ChatUser[] chatUsers;

    // Burst - temporarily set lower maximum value for message spawn timer
    private bool burstActive;
    private float burstTimer;
    private float burstTimeout = 3f;

    private float newMessageTimer;
    private float newMessageTimeout;
    [SerializeField] private float timerFloor = .01f;
    [SerializeField] private float timerCeilingNormal = 1f;
    [SerializeField] private float timerCeilingBurst = .15f;

    [SerializeField] private int maxEmotesInMessage = 4;
    [SerializeField] private float sameEmoteSpamChance = .7f;

    [SerializeField] private int maxMessagesOnScreen = 13;

    private List<GameObject> messageGameObjects;

    private MessageType currentMessageType;

    private void Awake()
    {
        messageTemplate.SetActive(false);
        currentMessageType = MessageType.Any;
    }

    private void Start()
    {
        Instance = this;

        newMessageTimer = 0f;
        burstTimer = 0f;

        messageGameObjects = new();

        goodEmoteSpritesArray = Resources.LoadAll("Emotes/Good", typeof(Sprite));
        badEmoteSpritesArray = Resources.LoadAll("Emotes/Bad", typeof(Sprite));
        otherEmoteSpritesArray = Resources.LoadAll("Emotes/Other", typeof(Sprite));

        allEmoteSpritesArray = goodEmoteSpritesArray.Union(badEmoteSpritesArray).ToArray();
        allEmoteSpritesArray = allEmoteSpritesArray.Union(otherEmoteSpritesArray).ToArray();

        TextAsset chatUsersTextFile = Resources.Load<TextAsset>("chat_names");
        chatUsers = JsonConvert.DeserializeObject<ChatUser[]>(chatUsersTextFile.text);
    }

    private void Update()
    {
        UpdateBurstTimer();
        UpdateTimer();

        //TODO: remove testing code
        if (Input.GetKeyDown(KeyCode.L)) ActivateBurst(MessageType.Good);
        if (Input.GetKeyDown(KeyCode.U)) ActivateBurst(MessageType.Bad);
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

        SpawnRandomMessage(currentMessageType);
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

        currentMessageType = MessageType.Any;
        OnBurstEnd?.Invoke();
    }

    public void SpawnRandomMessage(MessageType type = MessageType.Any)
    {
        GameObject messageObject = Instantiate(messageTemplate, messageContainer.transform);

        var chatMessage = messageObject.GetComponent<ChatMessage>();
        messageObject.SetActive(true);

        ChatUser chatUser = GetRandomChatUser();
        var color = chatUser.color;

        chatMessage.SetChatterName(chatUser.username, color["red"], color["green"], color["blue"]);

        int emotesInThisMessage = UnityEngine.Random.Range(1, maxEmotesInMessage);
        bool sameEmoteSpam = UnityEngine.Random.Range(0f, 1f) > sameEmoteSpamChance;

        Sprite emoteSprite = GetRandomEmote(type);
        for (int i = 0; i < emotesInThisMessage; i++)
        {
            chatMessage.AddEmote(emoteSprite);
            if (!sameEmoteSpam) emoteSprite = GetRandomEmote(type);
        }

        // Track and cull excess messages
        messageGameObjects.Add(messageObject);
        if (messageGameObjects.Count > maxMessagesOnScreen)
        {
            Destroy(messageGameObjects[0]);
            messageGameObjects.RemoveAt(0);
        }
    }

    private ChatUser GetRandomChatUser()
    {
        int randomIndex = UnityEngine.Random.Range(0, chatUsers.Length);
        return chatUsers[randomIndex];
    }

    private Sprite GetRandomEmote(MessageType type)
    {
        UnityEngine.Object[] currentArray;

        if (type == MessageType.Good) currentArray = goodEmoteSpritesArray;
        else if (type == MessageType.Bad) currentArray = badEmoteSpritesArray;
        else currentArray = allEmoteSpritesArray;

        int randomIndex = UnityEngine.Random.Range(0, currentArray.Length);
        return currentArray[randomIndex] as Sprite;
    }

    // <summary>
    // Call this to make chat go mental.
    // Optionally takes a message type to determine which emotes should be used,
    // And a float for duration of burst.
    // </summary>
    public void ActivateBurst(MessageType messageType = MessageType.Good, float burstTimeout = 1.5f)
    {
        currentMessageType = messageType;

        burstActive = true;
        newMessageTimeout = 0.1f; // Don't wait for timer to expire before bursting
        this.burstTimeout = burstTimeout;

        OnBurstStart?.Invoke();
    }

    private class ChatUser
    {
        public string username;
        public Dictionary<string, byte> color;
        public bool sub;
    }
}
