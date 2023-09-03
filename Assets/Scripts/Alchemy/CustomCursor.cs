using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class literally just makes our custom cursour follow the mouse.
/// Yeah.
/// Active when an ingredient is being dragged.
/// </summary>
public class CustomCursor : MonoBehaviour
{
    // we need this Awake() because update takes effect on the *next* frame, so
    // on the first frame after the game object activates, the position hasn't updated yet
    private void Awake()
    {
        transform.position = Input.mousePosition;
    }
    void Update()
    {
        transform.position = Input.mousePosition;
    }
}
