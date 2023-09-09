using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinUI : MonoBehaviour
{
    [SerializeField] private Image winImage;
    public static WinUI instance;

    private void Start()
    {
        instance = this;
        winImage.color = new Color(0, 0, 0, 0);

        winImage.gameObject.SetActive(false);
    }

    public void FadeInThis()
    {
        winImage.gameObject.SetActive(true);

        StartCoroutine(FadeImage(winImage, false));
    }

    // shamelessly stolen
    IEnumerator FadeImage(Image image, bool fadeAway, float seconds = 5)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            for (float i = seconds; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                image.color = new Color(1, 1, 1, i / seconds);
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= seconds; i += Time.deltaTime)
            {
                // set color with i as alpha
                image.color = new Color(1, 1, 1, i / seconds);
                yield return null;
            }
        }
    }
}
