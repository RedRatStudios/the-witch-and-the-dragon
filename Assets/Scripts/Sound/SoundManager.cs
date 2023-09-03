using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource musicSource, effectsSource;
    [SerializeField] private AudioMixer audioMixer;

    private Vector3 position;
    private float volumeMultiplier = 30f;

    // storing in a hashmap to prevent float innacuracy bullshit
    private Dictionary<SoundMixer.Groups, float> normalizedValues = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Make sure music persists between scene changes
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        InitializeValues();
    }

    private void Start()
    {
        position = Camera.main.transform.position;

        // subscribe to events here
    }

    private void InitializeValues()
    {
        // Load volume settings or set to defaults 
        normalizedValues.Add(SoundMixer.Groups.Master,
                             PlayerPrefs.GetFloat(SoundMixer.Groups.Master.ToString(), 1f));

        normalizedValues.Add(SoundMixer.Groups.Music,
                             PlayerPrefs.GetFloat(SoundMixer.Groups.Music.ToString(), 1f));

        normalizedValues.Add(SoundMixer.Groups.Effects,
                             PlayerPrefs.GetFloat(SoundMixer.Groups.Effects.ToString(), 1f));
    }

    public void PlaySoundEffect(AudioClip audio)
    {

    }

    public void SetGroupVolume(float volumeNormalized, SoundMixer.Groups group)
    {
        normalizedValues[group] = volumeNormalized;

        // converting from 0.0f~1.0f to logarhithmic scale
        float realVolume = Mathf.Log10(volumeNormalized) * volumeMultiplier;

        audioMixer.SetFloat(group.ToString(), realVolume);
    }

    public float GetGroupVolumeReal(SoundMixer.Groups group)
    {
        audioMixer.GetFloat(group.ToString(), out float volume);
        return volume;
    }

    public float GetGroupVolumeNormalized(SoundMixer.Groups group) => normalizedValues[group];
}