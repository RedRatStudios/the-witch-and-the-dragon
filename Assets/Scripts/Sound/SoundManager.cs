using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance { get; private set; }
    public static List<AudioSource> LoopingAudioSources { get; private set; }

    [SerializeField] private AudioSource musicSource, musicSourceSecondary, effectsSource;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioRef audioRef;
    [SerializeField] private MusicRef musicRef;

    private Vector3 position;
    private float volumeMultiplier = 30f;

    // storing normalized volume in a hashmap to prevent float innacuracy bullshit
    private Dictionary<SoundMixer.Groups, float> normalizedVolumeValues = new();

    private void Awake()
    {
        Instance = this;
        LoopingAudioSources = new();

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
        {
            PlaySoundEffect(audioRef.clank);
            PlaySoundEffect(audioRef.clank, loop: true);
            PlaySoundEffect(audioRef.loop_boil);
        }

        if (Input.GetKeyDown(KeyCode.L))
            PlayMusic(null);

        if (Input.GetKeyDown(KeyCode.U))
            PlayMusic(musicRef.megalovaniaLoop, musicRef.megalovaniaIntro);

        if (Input.GetKeyDown(KeyCode.Y))
        {
            musicSource.time = musicSource.clip.length / 10 * 9;
            musicSourceSecondary.time = musicSource.clip.length / 10 * 9;
        }
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

    // <summary>
    // Play a sound effect from audioRef scriptable object.
    // Opitonally, pass an object to spawn the sound at.
    // This one handles cases with multiple sound effects
    // </summary>
    public void PlaySoundEffect(AudioClip[] audioArray, bool loop = false, GameObject audioOrigin = null, float volume = 1f)
    {
        int clipIndex = UnityEngine.Random.Range(0, audioArray.Length);
        PlaySoundEffect(audioArray[clipIndex], loop, audioOrigin, volume);
    }

    // <summary>
    // Play a sound effect from audioRef scriptable object.
    // Opitonally, pass an object to spawn the sound at.
    // </summary>
    public void PlaySoundEffect(AudioClip audioClip, bool loop = false, GameObject audioOrigin = null, float volume = 1f)
    {
        if (audioOrigin == null)
            audioOrigin = Camera.main.gameObject;

        AudioSource source = audioOrigin.AddComponent<AudioSource>();

        // I really don't like Unity's sound management.
        source.outputAudioMixerGroup = audioMixer.FindMatchingGroups(SoundMixer.Groups.Effects.ToString())[0];

        source.loop = loop;
        source.clip = audioClip;
        source.volume = volume;
        source.Play();

        if (!loop)
            StartCoroutine(SourceDestroyer9000(source, audioClip.length));
        else
            // Not cleaning up forever looping sources just yet, so just store them for later reference
            LoopingAudioSources.Add(source);
    }

    private IEnumerator SourceDestroyer9000(AudioSource audioOrigin, float delay)
    {
        yield return new WaitForSeconds(delay + 1f);
        Destroy(audioOrigin);
    }

    // <summary>
    // Play a music loop or a loop+intro combination.
    // </summary>
    private void PlayMusic(AudioClip musicLoop, AudioClip musicIntro = null, float volume = 1f, bool fadeout = true)
    {
        // TODO: implement this properly
        // TODO: layer switching

        if (fadeout)
        {
            // TODO: fade in / out
        }

        musicSource.Stop();
        musicSourceSecondary.Stop();

        if (musicIntro == null)
        {
            musicSource.clip = musicLoop;
            musicSource.loop = true;

            if (musicLoop != null)
                musicSource.Play();
            return;
        }

        if (musicLoop == null)
        {
            Debug.LogError("Provided an intro but not a loop.");
            return;
        }

        // Play out the intro if provided, schedule loop to play
        musicSource.clip = musicIntro;
        musicSource.loop = false;

        musicSourceSecondary.clip = musicLoop;
        musicSourceSecondary.loop = true;

        // precise calculation because clip.length lies to you
        double duration = (double)musicIntro.samples / musicIntro.frequency;

        double delay = 0.05d; // hack to stop some weirdness
        musicSource.PlayScheduled(AudioSettings.dspTime + delay);
        musicSourceSecondary.PlayScheduled(AudioSettings.dspTime + duration + delay);
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