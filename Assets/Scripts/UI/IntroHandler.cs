using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroHandler : MonoBehaviour
{
    [SerializeField] AudioClip audio;
    private AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.volume = .5f;

        StartCoroutine(PlayIntro());
        StartCoroutine(SwitchScene());
    }

    private IEnumerator SwitchScene()
    {
        yield return new WaitForSeconds(4.5f);
        SceneManager.LoadScene("MainMenuScene");

    }

    IEnumerator PlayIntro()
    {
        yield return new WaitForSeconds(0.2f);
        source.PlayOneShot(audio);
    }
}
