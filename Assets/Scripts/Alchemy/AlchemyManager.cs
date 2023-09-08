using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;


// Handles storing and converting ingredients
public class AlchemyManager : MonoBehaviour
{
    public static AlchemyManager Instance;

    public static event Action<Dictionary<string, string>> OnIngredientsCombined;

    public static event Action OnSpicyMessageSent;
    public static event Action OnFunnyMessageSent;
    public static event Action OnBadMessageSent;
    public static event Action OnOKMessageSent;

    public static event Action OnCookingCooked;

    [SerializeField] private TextAsset ingredientsFile;
    [SerializeField] private TextAsset messageDataFile;

    [SerializeField] private PROTOTYPE_ButtonUI buttonUI;

    private Dictionary<string, string[]> ingredientsDict;
    private Dictionary<string, Dictionary<string, string>> messageDataDict;

    // Potentially store these in a list if we want to combine more than two
    private string choiceOne;
    private string choiceTwo;

    private bool cooking;
    private float cookingTimer;
    public float cookingTimerCeiling = 1.5f;

    private void Awake()
    {
        Instance = this;

        // load dictionary data from jsons
        ingredientsDict = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(ingredientsFile.text);
        messageDataDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(messageDataFile.text);
    }

    private void Start()
    {
        // TODO: remove testing code
        OnIngredientsCombined += result => { Debug.Log(result["message"]); };
        OnIngredientsCombined += result => { Debug.Log($"funny: {result["funny"]}"); };
        OnIngredientsCombined += result => { Debug.Log($"annoying: {result["annoying"]}"); };
        OnIngredientsCombined += result => { Debug.Log($"topic: {result["topic"]}"); };

        buttonUI.PROTOTYPE_InitButtons();
    }

    private void Update()
    {
        if (!cooking) return;
        cookingTimer += Time.deltaTime * PlayerStats.Instance.cookingTimeMultiplier;

        if (cookingTimer < cookingTimerCeiling) return;
        cookingTimer = 0f;
        cooking = false;
        CombineIngredients();
    }

    // <summary>
    // Adds ingredient to slot one or slot two. Currently also calls combine at the end,
    // but that could be done on demand instead.
    // </summary>
    public void AddIngredient(string ingredient)
    {
        if (choiceOne == null)
        {
            choiceOne = ingredient;
            return;
        }

        // sanity check
        if (choiceTwo == null)
        {
            choiceTwo = ingredient;
            cooking = true;
            return;
        }

        Debug.LogError("Called AddIngredient() when both ingredient slots are filled but haven't yet been combined or cleaned.");
    }

    public void ClearIngredients()
    {
        choiceOne = null;
        choiceTwo = null;
    }

    // <summary>
    // Combine the ingredients, handle edge cases, clean both slots.
    // On finishing, invokes OnIngredientsCombined with full message properties as parameter.
    // As well as a couple more specific events.
    // </summary>
    public void CombineIngredients()
    {
        string[] resultsOne = ingredientsDict[choiceOne];
        string[] resultsTwo = ingredientsDict[choiceTwo];
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
        choiceOne = null;
        choiceTwo = null;

        var messageData = messageDataDict[outputResult];

        if (PlayerStats.Instance.highlithedMessage)
        {
            PlayerStats.Instance.highlithedMessage = false;
            Upgrade.Highlight.locked = false;

            messageData["funny"] = (Convert.ToDouble(messageData["funny"]) * 10).ToString();
            messageData["annoying"] = (Convert.ToDouble(messageData["annoying"]) * 10).ToString();
        }

        OnIngredientsCombined?.Invoke(messageData);

        double funny = Convert.ToDouble(messageData["funny"]);
        double annoying = Convert.ToDouble(messageData["annoying"]);

        if (funny >= .6)
        {
            if (annoying >= funny - .25)
                OnSpicyMessageSent?.Invoke();
            else
                OnFunnyMessageSent?.Invoke();

            return;
        }

        if (annoying >= funny + .25)
        {
            OnBadMessageSent?.Invoke();
            return;
        }

        OnOKMessageSent?.Invoke();
    }

    public float GetCookingProgress() => cookingTimer / cookingTimerCeiling;

    // <summary>
    // Get all available ingredients as a List<string>
    // </summary>
    public List<string> GetIngredients()
    {
        List<string> output = new();

        foreach (string ingredient in ingredientsDict.Keys)
        { output.Add(ingredient); }

        return output;
    }

    public Dictionary<string, Dictionary<string, string>> MessageDataDictionary => messageDataDict;
    public Dictionary<string, string[]> IngredientDictionary => ingredientsDict;
}