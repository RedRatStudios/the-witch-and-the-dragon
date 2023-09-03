using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Alchemy
{
    private readonly IngredientList ingredients;

    // Every new instance of Alchemy will read the JSON again, 
    // if this class is used in more than one script we should probably do this differently
    public Alchemy(string json) {
        ingredients = JsonConvert.DeserializeObject<IngredientList>(json);
    }

    /// <summary>
    /// This method gets the name of all ingredients available and returns a list of it.
    /// </summary>
    public List<string> GetIngredients()
    {
        return new List<string>(ingredients.list.Keys);
    }

    /// <summary>
    /// This method finds a combination of two ingredients and gets a random setence
    /// from the combination array.
    /// </summary>
    public string Combine(string firstEle, string secondEle)
    {
        try
        {
            string[] results = ingredients.list[firstEle].combinations[secondEle];
            int randomIndex = Random.Range(0, results.Length);

            return results[randomIndex];
        }
        catch (KeyNotFoundException e)
        {
            return "The ingredient you provided was not found: " + e;
        }
    }
}

public class IngredientList
{
    public Dictionary<string, Ingredient> list;
}
