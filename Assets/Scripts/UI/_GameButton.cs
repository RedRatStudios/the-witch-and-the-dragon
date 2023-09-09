using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class _GameButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static event Action OnButtonPressed;
    public static event Action OnButtonReleased;

    protected Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnButtonPressed?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnButtonReleased?.Invoke();
    }
}
