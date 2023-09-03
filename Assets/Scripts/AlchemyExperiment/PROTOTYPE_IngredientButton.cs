using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PROTOTYPE_IngredientButton : MonoBehaviour
{
    // Stores ingredient name, and sends it to AlchemyManager on click

    [SerializeField] private string PROTOTYPE_ingredient;

    void Start()
    {
        Button button = gameObject.GetComponent<Button>();

        button.onClick.AddListener(() =>
        {
            AlchemyManager.Instance.PROTOTYPE_AddIngredient(PROTOTYPE_ingredient);
        });
    }

}
