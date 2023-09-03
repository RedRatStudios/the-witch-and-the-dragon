using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Alchemy : MonoBehaviour
{
    [SerializeField]
    private TextAsset jsonFile;
    private IngredientList ingredients;

    void Start()
    {
        ingredients = JsonConvert.DeserializeObject<IngredientList>(jsonFile.text);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Trying to read JSON");
            Debug.Log("Number of Ingredients: " + ingredients.list.Count );
        }
    }
}

public class IngredientList
{
    public Dictionary<string, Ingredient> list;
}

public class Ingredient
{
    public Dictionary<string, string[]> combinations;
}