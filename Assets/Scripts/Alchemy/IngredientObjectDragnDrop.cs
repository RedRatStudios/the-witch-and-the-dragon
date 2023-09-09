using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngredientObject : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // Stores ingredient name, and sends it to AlchemyManager on click
    [SerializeField] public Ingredient ingredient;
    private Image image;
    private Vector3 originalpos;
    private CauldronSlot nearestSlot;

    void Start()
    {
        image = gameObject.GetComponent<Image>();
        originalpos = gameObject.transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        nearestSlot = null;
        image.sprite = ingredient.spriteIcon;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float shortestDistance = 80;
        foreach (var slot in CauldronSlot.AllActiveSlots)
        {
            float dist = Vector2.Distance(eventData.position, slot.transform.position);

            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                nearestSlot = slot;
            }
        }

        // if there was a slot near enough to the mouse,
        // put the appropiate ingredient in it.
        if (nearestSlot == null)
        {
            transform.position = originalpos;
            image.sprite = ingredient.spriteList;
            return;
        }

        if(!AlchemyManager.Instance.AddIngredient(ingredient)){
            transform.position = originalpos;
            image.sprite = ingredient.spriteList;
            return;
        }

        image.enabled = false;
    }
}