using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Video;
using UnityEngine.UI;

public class PROTOTYPE_IngredientButton : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // Stores ingredient name, and sends it to AlchemyManager on click
    [SerializeField] public Ingredient ingredient;
    private Vector2 defaultTransform;
    private CauldronSlot nearestSlot;

    void Start()
    {
        Button button = gameObject.GetComponent<Button>();
        defaultTransform = transform.position;

        // button.onClick.AddListener(() =>
        // {
        //     AlchemyManager.Instance.AddIngredient(ingredient);
        // });
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        nearestSlot = null;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float shortestDistance = 50;
        foreach (var slot in CauldronSlot.AllActiveSlots)
        {
            float dist = Vector2.Distance(eventData.position, slot.transform.position);

            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                nearestSlot = slot;
            }
        }

        transform.position = defaultTransform;

        // if there was a slot near enough to the mouse,
        // put the appropiate ingredient in it.
        if (nearestSlot == null) return;

        AlchemyManager.Instance.AddIngredient(ingredient);
    }
}