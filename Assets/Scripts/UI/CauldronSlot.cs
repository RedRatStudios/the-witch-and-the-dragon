using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CauldronSlot : MonoBehaviour
{
    public static List<CauldronSlot> AllActiveSlots = new();

    [SerializeField] private Sprite defaultSprite;
    private Ingredient ingredient = null;

    public void SetIngredient(Ingredient ingredient)
    {
        Debug.Log("setting");
        this.ingredient = ingredient;
        SetSprite(ingredient.sprite);
        AllActiveSlots.Add(this);
    }

    public void SetSprite(Sprite sprite = null)
    {
        if (sprite == null)
            sprite = defaultSprite;
        GetComponent<Image>().sprite = sprite;
    }

    public bool HasIngredient() => ingredient != null;
    public void Clear()
    {
        ingredient = null;
        SetSprite(null);
    }
    public Ingredient Ingredient => ingredient;
}