using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;


// Handles storing and converting ingredients
public class AlchemyManager : MonoBehaviour
{
    public static AlchemyManager Instance;

    public static event Action<Message> OnIngredientsCombinedResultingMessage;

    public event Action OnSpicyMessageSent;
    public event Action OnFunnyMessageSent;
    public event Action OnBadMessageSent;
    public event Action OnOKMessageSent;

    public static event Action OnCookingCooked;

    private static UnityEngine.Object[] ingredientsListImg;
    private static UnityEngine.Object[] ingredientsIconImg;


    [SerializeField] private TextAsset ingredientsFile;
    [SerializeField] private TextAsset messageDataFile;
    [SerializeField] private SpriteRef spriteRef;

    [SerializeField] private IngredientObjectContainer buttonUI;

    private Dictionary<string, string[]> ingredientsDict;
    private Dictionary<string, Dictionary<string, string>> messageDataDict;

    private List<Ingredient> allIngredients;

    // Potentially store these in a list if we want to combine more than two
    [SerializeField] private CauldronSlot slotOne;
    [SerializeField] private CauldronSlot slotTwo;

    private bool cooking;
    private float cookingTimer;
    public float cookingTimerCeiling = 1.5f;

    private void Awake()
    {
        Instance = this;
        allIngredients = new();
        CauldronSlot.AllActiveSlots.Add(slotOne);
        CauldronSlot.AllActiveSlots.Add(slotTwo);

        // load dictionary data from jsons
        ingredientsDict = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(ingredientsFile.text);
        messageDataDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(messageDataFile.text);

        // Load component sprites
        ingredientsIconImg =  Resources.LoadAll("Ingredients/Icon", typeof(Sprite));
        ingredientsListImg = Resources.LoadAll("Ingredients/Listing", typeof(Sprite));
    }

    private void Start()
    {
        // Populate ingredient list
        foreach (string str in ingredientsDict.Keys)
        {
            allIngredients.Add(new Ingredient()
            {
                ingredientName = str,
                messages = ingredientsDict[str],
                sprite = spriteRef.dictionary[str],
            });
        }

        // Spawn initial hand
        IngredientObjectContainer.Instance.GenerateNewIngredientObjects(ingredientsIconImg, ingredientsListImg);

        // TODO: remove testing code
        OnIngredientsCombinedResultingMessage += message => Debug.Log(
            $"{message.content} \n {message.funny}\n"
            + $"{message.annoying}\n {message.topic}"
            );
    }



    private void Update()
    {
        UpdateCooking();
    }

    private void UpdateCooking()
    {
        if (!cooking) return;
        cookingTimer += Time.deltaTime * PlayerStats.Instance.cookingTimeMultiplier;

        if (cookingTimer < cookingTimerCeiling) return;
        cookingTimer = 0f;
        cooking = false;
        CombineIngredients();
    }

    // <summary>
    // Adds ingredient to slot one or slot two. Currently also starts cooking,
    // but that could be done on demand instead.
    // </summary>
    public void AddIngredient(Ingredient ingredient)
    {
        if (!slotOne.HasIngredient())
        {
            slotOne.SetIngredient(ingredient);
            slotOne.SetSprite(ingredient.sprite);
            return;
        }

        if (!slotTwo.HasIngredient())
        {
            slotTwo.SetIngredient(ingredient);
            slotTwo.SetSprite(ingredient.sprite);

            // This is where the cooking begins
            cooking = true;
            return;
        }

        Debug.LogError("Called AddIngredient() when both ingredient slots are filled but haven't yet been combined or cleaned.");
    }

    // <summary>
    // Combine the ingredients, handle edge cases, clean both slots.
    // On finishing, invokes OnIngredientsCombined with full message properties as parameter.
    // As well as a couple more specific events.
    // </summary>
    public void CombineIngredients()
    {
        string[] resultsOne = slotOne.Ingredient.messages;
        string[] resultsTwo = slotTwo.Ingredient.messages;
        string outputResult;

        if (resultsOne == resultsTwo)
        {
            // Same ingredient, default to first message
            outputResult = resultsOne[0];
            goto FinishCombining;
        }

        List<string> finalResults = new();

        // Find matches
        foreach (string result in resultsOne)
        {
            foreach (string secondResult in resultsTwo)
            { if (result == secondResult) finalResults.Add(result); }
        }

        if (finalResults.Count == 0)
        {
            Debug.Log("No combinations available for these ingredients.");
            outputResult = resultsOne[0]; // default to first message
            goto FinishCombining;
        }

        // We have a result, pick random if there's more than one
        int randomIndex = UnityEngine.Random.Range(0, finalResults.Count);
        outputResult = finalResults[randomIndex];

    FinishCombining:
        slotOne.Clear();
        slotTwo.Clear();
        IngredientObjectContainer.Instance.GenerateNewIngredientObjects(ingredientsIconImg, ingredientsListImg);

        var messageData = messageDataDict[outputResult];
        Message message = new(messageData["message"],
                              messageData["topic"],
                              (float)Convert.ToDouble(messageData["funny"]),
                              (float)Convert.ToDouble(messageData["annoying"]));

        if (PlayerStats.Instance.highlithedMessage)
        {
            PlayerStats.Instance.highlithedMessage = false;
            Upgrade.Highlight.locked = false;
            message.funny *= 6;
            message.annoying *= 2;
        }

        OnIngredientsCombinedResultingMessage?.Invoke(message);

        if (message.funny >= .6)
        {
            if (message.annoying >= message.funny - .25)
                OnSpicyMessageSent?.Invoke();
            else
                OnFunnyMessageSent?.Invoke();

            return;
        }

        if (message.annoying >= message.funny + .25)
        {
            OnBadMessageSent?.Invoke();
            return;
        }

        OnOKMessageSent?.Invoke();
    }

    public float GetCookingProgress() => cookingTimer / cookingTimerCeiling;

    public List<Ingredient> AllIngredients => allIngredients;
    public Dictionary<string, Dictionary<string, string>> MessageDataDictionary => messageDataDict;
    public Dictionary<string, string[]> IngredientDictionary => ingredientsDict;
}

public class Message
{
    public string content;
    public string topic;
    public float funny;
    public float annoying;

    public Message(string content, string topic, float funny, float annoying)
    {
        this.content = content;
        this.topic = topic;
        this.funny = funny;
        this.annoying = annoying;
    }
}