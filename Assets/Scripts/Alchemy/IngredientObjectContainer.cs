using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

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

            // TODO: set UI values when UI is done
            ingredientObject.GetComponentInChildren<TextMeshProUGUI>().text = $"Add {ingredient.ingredientName}";

            ingredientObject.GetComponent<IngredientObject>().ingredient = ingredient;
            ingredientObject.SetActive(true);
            allObjects.Add(ingredientObject);
        }
    }
}