using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PROTOTYPE_IngredientButton : MonoBehaviour
{
    // Stores ingredient name, and sends it to AlchemyManager on click
    [SerializeField] public string ingredient;

    void Start()
    {
        Button button = gameObject.GetComponent<Button>();

        button.onClick.AddListener(() =>
        {
            AlchemyManager.Instance.AddIngredient(ingredient);
        });
    }

}
