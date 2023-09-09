using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class IngredientObjectContainer : MonoBehaviour
{
    public static IngredientObjectContainer Instance;
    private static List<GameObject> allObjects = new();

    [SerializeField] GameObject objectTemplate;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        objectTemplate.SetActive(false);

    }

    public void GenerateNewIngredientObjects(int number = 4)
    {
        // clean up
        foreach (var gameObject in allObjects)
            Destroy(gameObject);
        allObjects.Clear();

        // spawn
        for (int i = 0; i < number; i++)
        {
            int randomIndex = Random.Range(0, AlchemyManager.Instance.AllIngredients.Count);
            Ingredient ingredient = AlchemyManager.Instance.AllIngredients[randomIndex];

            GameObject ingredientObject = Instantiate(objectTemplate, transform);

            // Find the sprite that matches the random ingredient
            ingredientObject.GetComponentInChildren<Image>().sprite = ingredient.spriteList;

            ingredientObject.GetComponent<IngredientObject>().ingredient = ingredient;
            ingredientObject.SetActive(true);
            allObjects.Add(ingredientObject);
        }
    }
}