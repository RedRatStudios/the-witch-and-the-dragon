using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class just manages the drag n' drop effect of ingredients onto the pot.
/// This and Alchemy.cs will probably be merged in future.
/// </summary>
public class AlchemyManager : MonoBehaviour
{
    [SerializeField]
    private TextAsset ingredientsJsonFile;
    private Alchemy alchemy;
    // currentIngredient stores the ingredient currently being dragged with the mouse
    private Ingredient currentIngredient;
    // customCursor is simply the image that activates while dragging an ingredient
    public Image customCursor;
    public TextMeshProUGUI resultText;
    // slots in the pot, which is totally not a cauldron, rather a pot. 
    // rename it to something clearer.
    public CookingSlot[] cookingSlots;

    void Start()
    {
        alchemy = new(ingredientsJsonFile.text);
    }

    private void Update()
    {
        // The "on mouse release" side of the drag n' drop aspect.
        // Disables customCursor & puts the ingredient in the pot where applicable.
        if (Input.GetMouseButtonUp(0)){
            if(currentIngredient != null){
                // activates the image to show that you're dragging an ingredient
                customCursor.gameObject.SetActive(false);
                CookingSlot nearestSlot = null;

                // this value determines how far we can drop an ingredient from the
                // pot slot for it to actually snap in place
                float shortestDistance = 50;

                // checks for the closest slot to our mouse within shortestDistance
                foreach(CookingSlot slot in cookingSlots){
                    float dist = Vector2.Distance(Input.mousePosition, slot.transform.position);

                    if(dist < shortestDistance){
                        shortestDistance = dist;
                        nearestSlot = slot;
                    }
                }

                // if there was a slot near enough to the mouse,
                // put the appropiate ingredient in it.
                if (nearestSlot == null){
                    currentIngredient = null;
                }
                else
                {
                    nearestSlot.gameObject.SetActive(true);
                    nearestSlot.GetComponent<Image>().sprite = currentIngredient.GetComponent<Image>().sprite;
                    nearestSlot.ingredient = currentIngredient;
                    currentIngredient = null;
                }
            }
        }
    }

    /// <summary>
    /// This method checks the ingredients in the pot, and gives the appropiate recipe result.
    /// </summary>
    public void OnRecipeMade()
    {   
        // check that there's actually an ingredient in both slots of the pot
        if (cookingSlots[0].ingredient == null || cookingSlots[1].ingredient == null){
            resultText.text = "not a valid recipe lmao";
            return;
        }
        // if so, Combine() them. doesn't actually check if it's a valid combination yet.
        resultText.text = alchemy.Combine(cookingSlots[0].ingredient.ingredientName, cookingSlots[1].ingredient.ingredientName);
    }

    /// <summary>
    /// This method runs when you click an ingredient. Checks hwat ingredient it is, 
    /// and lets you drag it by assigning and activating customCursor
    /// </summary>
    public void OnMouseDownIngredient(Ingredient ingredient)
    {
        // if statement should be redundant, but it's here in case something else fucked up
        if(currentIngredient == null){
            currentIngredient = ingredient;
            // turns on the customCursor and gives it the appropiate ingredient image
            customCursor.gameObject.SetActive(true);
            customCursor.sprite = currentIngredient.GetComponent<Image>().sprite;
        }
    }
}
