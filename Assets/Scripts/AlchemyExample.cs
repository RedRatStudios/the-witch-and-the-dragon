using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemyExample : MonoBehaviour
{
    [SerializeField]
    private TextAsset ingredientsJsonFile;
    private Alchemy alchemy;

    void Start()
    {
        alchemy = new(ingredientsJsonFile.text);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            PrintIngredients(alchemy.GetIngredients());
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log(alchemy.Combine("Horny", "Weeb"));
        }
    }

    private void PrintIngredients(List<string> ingredients)
    {
        foreach (string ingredient in ingredients)
        {
            Debug.Log(ingredient);
        }
    }
}