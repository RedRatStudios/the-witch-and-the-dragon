using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchEmotion : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        AngerManager.OnAngerChange += UpdateEmotion;
    }

    private void UpdateEmotion(float value)
    {
        anim.SetFloat("Intensity", value);
    }
}
