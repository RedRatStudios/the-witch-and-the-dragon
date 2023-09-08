using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PROTOTYPE_ButtonUI : MonoBehaviour
{
    [SerializeField] GameObject buttonTemplate;

    private void Start()
    {
        buttonTemplate.SetActive(false);
    }

    public void PROTOTYPE_InitButtons()
    {
        foreach (Ingredient ingredient in AlchemyManager.Instance.GetIngredients())
        {
            GameObject button = Instantiate(buttonTemplate, transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = $"Add {ingredient}";
            button.GetComponent<PROTOTYPE_IngredientButton>().ingredient = ingredient;
            button.SetActive(true);
        }
    }
}
