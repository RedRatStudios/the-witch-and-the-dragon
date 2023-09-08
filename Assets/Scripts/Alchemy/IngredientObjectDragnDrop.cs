using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngredientObject : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // Stores ingredient name, and sends it to AlchemyManager on click
    [SerializeField] public Ingredient ingredient;
    private Button button;
    private Image image;
    private TextMeshProUGUI textMesh;
    private CauldronSlot nearestSlot;

    void Start()
    {
        button = gameObject.GetComponent<Button>();
        image = gameObject.GetComponent<Image>();
        textMesh = gameObject.GetComponentInChildren<TextMeshProUGUI>();
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
        float shortestDistance = 70;
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
        if (nearestSlot == null) return;

        AlchemyManager.Instance.AddIngredient(ingredient);
        image.enabled = false;
        textMesh.enabled = false;
    }
}