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
    [SerializeField] private AudioRef audioRef;
    [SerializeField] private MusicRef musicRef;

    private Vector3 position;
    private float volumeMultiplier = 30f;

    // storing in a hashmap to prevent float innacuracy bullshit
    private Dictionary<SoundMixer.Groups, float> normalizedVolumeValues = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        // Load volume settings or set to defaults 
        foreach (SoundMixer.Groups group in Enum.GetValues(typeof(SoundMixer.Groups)))
        {
            normalizedVolumeValues.Add(group, PlayerPrefs.GetFloat(group.ToString(), 1f));
        }
    }

    private void Start()
    {
        position = Camera.main.transform.position;
        foreach (SoundMixer.Groups group in Enum.GetValues(typeof(SoundMixer.Groups)))
        {
            SetGroupVolume(normalizedVolumeValues[group], group);
        }

        // subscribe to events here, use PlaySoundEffect().
        // i.e.
        // Alchemy.OnIngredientAdded += PlaySoundEffect(audioRef.ingredient);
    }

    // TODO: remove testing code
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            PlaySoundEffect(audioRef.clank);

        if (Input.GetKeyDown(KeyCode.L))
            PlayMusic(null);

        if (Input.GetKeyDown(KeyCode.U))
            PlayMusic(musicRef.mainMenu);
    }

    private void OnDestroy()
    {
        // Save changes to persist between restarts
        foreach (SoundMixer.Groups group in Enum.GetValues(typeof(SoundMixer.Groups)))
        {
            PlayerPrefs.SetFloat(group.ToString(), normalizedVolumeValues[group]);
        }

        PlayerPrefs.Save();
    }

    // Handles cases with multiple sound effects
    public void PlaySoundEffect(AudioClip[] audioArray, AudioSource source = null, float volume = 1f)
    {
        int clipIndex = UnityEngine.Random.Range(0, audioArray.Length);
        PlaySoundEffect(audioArray[clipIndex], source, volume);
    }

    // <summary>
    // Play a sound effect from audioRef scriptable object.
    // Opitonally pass an object with AudioSource for spatial effects.
    // </summary>
    private void PlaySoundEffect(AudioClip audioClip, AudioSource source = null, float volume = 1f)
    {
        if (source == null) source = effectsSource;
        source.PlayOneShot(audioClip, volume);
    }

    // <summary>
    // Play music 
    // </summary>
    private void PlayMusic(AudioClip music, float volume = 1f)
    {
        // TODO: implement this properly
        // TODO: fade in / out
        // TODO: layer switching
        musicSource.Stop();

        musicSource.clip = music;
        musicSource.loop = true;

        if (music != null)
            musicSource.Play();
    }

    // <summary>
    // Set mixer volume for a given group using 0.0~1.0.
    // </summary>
    public void SetGroupVolume(float volumeNormalized, SoundMixer.Groups group)
    {
        normalizedVolumeValues[group] = volumeNormalized;

        // converting from 0.0f~1.0f to logarhithmic dB scale
        float realVolume = Mathf.Log10(volumeNormalized) * volumeMultiplier;
        audioMixer.SetFloat(group.ToString(), realVolume);
    }

    // <summary>
    // Get mixer volume for a given group in decibells.
    // </summary>
    public float GetGroupVolumeReal(SoundMixer.Groups group)
    {
        audioMixer.GetFloat(group.ToString(), out float volume);
        return volume;
    }

    // <summary>
    // Get mixer volume for a given group in 0.0~1.0 float.
    // </summary>
    public float GetGroupVolumeNormalized(SoundMixer.Groups group) => normalizedVolumeValues[group];
}