using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;


// Handles storing and converting ingredients
public class AlchemyManager : MonoBehaviour
{
    public static AlchemyManager Instance;
    public event Action<Dictionary<string, string>> OnIngredientsCombined;

    [SerializeField] private TextAsset ingredientsFile;
    [SerializeField] private TextAsset messageDataFile;

    [SerializeField] private PROTOTYPE_ButtonUI buttonUI;

    private Dictionary<string, string[]> ingredientsDict;
    private Dictionary<string, Dictionary<string, string>> messageDataDict;

    // Potentially store these in a list if we want to combine more than two
    private string choiceOne;
    private string choiceTwo;

    private void Awake()
    {
        Instance = this;

        // load dictionary data from jsons
        ingredientsDict = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(ingredientsFile.text);
        messageDataDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(messageDataFile.text);
    }

    void Start()
    {
        // TODO: remove testing code
        OnIngredientsCombined += result => { Debug.Log(result["message"]); };
        OnIngredientsCombined += result => { Debug.Log($"funny: {result["funny"]}"); };
        OnIngredientsCombined += result => { Debug.Log($"annoying: {result["annoying"]}"); };
        OnIngredientsCombined += result => { Debug.Log($"topic: {result["topic"]}"); };

        buttonUI.PROTOTYPE_InitButtons();
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
            CombineIngredients();
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
        OnIngredientsCombined?.Invoke(messageDataDict[outputResult]);
    }


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