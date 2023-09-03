using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;


// Handles storing and converting ingredients
public class AlchemyManager : MonoBehaviour
{
    public static AlchemyManager Instance;

    [SerializeField] private TextAsset ingredientsFile;

    private Dictionary<string, string[]> ingredientsDict;

    // Potentially store in an array if we want to combine more than two
    private string PROTOTYPE_choiceOne;
    private string PROTOTYPE_choiceTwo;

    void Start()
    {
        Instance = this;

        // Deserializing only once at start
        // Potentially make it static and public for outside scripts to access at any point
        ingredientsDict = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(ingredientsFile.text);
    }

    // Adds ingredient to slot one or slot two. Currently also calls combine at the end,
    // but that could be done on demand instead.
    public void PROTOTYPE_AddIngredient(string ingredient)
    {
        if (PROTOTYPE_choiceOne == null)
        {
            PROTOTYPE_choiceOne = ingredient;
            Debug.Log($"Added {ingredient} to choiceOne");
            return;
        }

        PROTOTYPE_choiceTwo = ingredient;
        Debug.Log($"Added {ingredient} to choiceTwo");
        PROTOTYPE_Combine();
    }

    // Cycle through both lists and find mathches, pick random match if there are multiple
    public void PROTOTYPE_Combine()
    {
        string[] resultsOne = ingredientsDict[PROTOTYPE_choiceOne];
        string[] resultsTwo = ingredientsDict[PROTOTYPE_choiceTwo];

        List<string> finalResults = new();

        foreach (string result in resultsOne)
        {
            foreach (string secondResult in resultsTwo)
            {
                if (result == secondResult) finalResults.Add(result);
            }
        }

        if (finalResults.Count == 0)
        {
            Debug.LogWarning("No combinations available for these ingredients.");
            goto NullingChoices;
        }

        // We have our result
        int randomIndex = UnityEngine.Random.Range(0, finalResults.Count);
        Debug.Log($"Combination result: {finalResults[randomIndex]}.");

    NullingChoices:
        PROTOTYPE_choiceOne = null;
        PROTOTYPE_choiceTwo = null;
    }
}