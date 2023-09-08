using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BanUI : MonoBehaviour
{
    [SerializeField] private Image banImage;
    [SerializeField] private Image background;

    private void Start()
    {
        banImage.color = new Color(0, 0, 0, 0);
        background.color = new Color(0, 0, 0, 0);

        banImage.gameObject.SetActive(false);
        background.gameObject.SetActive(false);

        AngerManager.Instance.OnMaxAnger += FadeInThis;
    }

    private void FadeInThis()
    {
        banImage.gameObject.SetActive(true);
        background.gameObject.SetActive(true);

        StartCoroutine(FadeImage(banImage, false));
        StartCoroutine(FadeImage(background, false));

        StartCoroutine(WaitAndFadeout(banImage));
        StartCoroutine(WaitAndFadeout(background));
    }

    IEnumerator WaitAndFadeout(Image image)
    {
        yield return new WaitForSeconds(5f);
        StartCoroutine(FadeImage(image, true));

        yield return new WaitForSeconds(4f);
        banImage.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
    }

    // shamelessly stolen
    IEnumerator FadeImage(Image image, bool fadeAway, float seconds = 4)
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
