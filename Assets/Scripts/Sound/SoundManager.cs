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
    [SerializeField] private UnityEngine.Audio.AudioMixer audioMixer;
    [SerializeField] private AudioRef audioRef;
    [SerializeField] private MusicRef musicRef;

    private Vector3 position;
    private float volumeMultiplier = 30f;

    // storing normalized volume in a hashmap to prevent float innacuracy bullshit
    private Dictionary<AudioMixer.Groups, float> normalizedVolumeValues = new();

    private void Awake()
    {
        Instance = this;
        LoopingAudioSources = new();

        // Load volume settings or set to defaults 
        foreach (AudioMixer.Groups group in Enum.GetValues(typeof(AudioMixer.Groups)))
            normalizedVolumeValues.Add(group, PlayerPrefs.GetFloat(group.ToString(), 1f));
    }

    private void Start()
    {
        position = Camera.main.transform.position;

        foreach (AudioMixer.Groups group in Enum.GetValues(typeof(AudioMixer.Groups)))
            SetGroupVolume(normalizedVolumeValues[group], group);

        // subscribe to events here, use PlaySoundEffect().
        ChatUI.OnMessageSpawned += () => PlaySoundEffect(audioRef.chatTyping, volume: 0.2f);

        AlchemyManager.OnBadMessageSent += () => PlaySoundEffect(audioRef.messageCooked_Bad);
        AlchemyManager.OnOKMessageSent += () => PlaySoundEffect(audioRef.messageCooked_OK);
        AlchemyManager.OnFunnyMessageSent += () => PlaySoundEffect(audioRef.messageCooked_Good);
        AlchemyManager.OnSpicyMessageSent += () => PlaySoundEffect(audioRef.messageCooked_Amazing);
    }

    // TODO: remove testing code
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlaySoundEffect(audioRef.messageCooked_Amazing);
            PlaySoundEffect(audioRef.loop_boil, true);
        }

        if (Input.GetKeyDown(KeyCode.Semicolon))
            PlayMusic(null);

        if (Input.GetKeyDown(KeyCode.U))
            PlayMusic(musicRef.megalovaniaLoop, musicRef.megalovaniaIntro);
    }

    private void OnDestroy()
    {
        DestroyLoopingSources();

        // Save changes to sound settings to persist between restarts
        foreach (AudioMixer.Groups group in Enum.GetValues(typeof(AudioMixer.Groups)))
            PlayerPrefs.SetFloat(group.ToString(), normalizedVolumeValues[group]);

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

        source.outputAudioMixerGroup = audioMixer.FindMatchingGroups(AudioMixer.Groups.Effects.ToString())[0];

        source.loop = loop;
        source.clip = audioClip;
        source.volume = volume;
        source.Play();

        if (!loop)
            StartCoroutine(SourceDestroyer9000(source, audioClip.length));
        else
            // Keeping track of looping sources to destroy them whenever soundmanager itself is destroyed.
            LoopingAudioSources.Add(source);
    }

    // HACK: idfk how exactly you're supposed to manage multiple sound effects in Unity
    // but you can't play all of them from the same source, and PlayOneShot() is
    // extremely restrictive: namely, it doesn't let you choose a mixer group.
    // So I'm just keeping track of all spawned sound sources and yeet them when their time comes.
    // I really don't like Unity's sound management.
    // also psd is not my favorite file format
    private IEnumerator SourceDestroyer9000(AudioSource audioOrigin, float delay)
    {
        yield return new WaitForSeconds(delay + 1f);
        Destroy(audioOrigin);
    }

    // <summary>
    // Play a music loop or a loop+intro combination.
    // </summary>
    public void PlayMusic(AudioClip musicLoop, AudioClip musicIntro = null, float volume = 1f, bool fadeout = true)
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

            musicSourceSecondary.clip = null;

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

        // HACK: delay to make sure the option that is literally SUPPOSED to play things SEAMLESSLY
        // has time to LOAD and COMPRESS the file before playing it so that it doesn't cause a GAP
        // because APPARENTLY it doesn't do it in advance.
        // what the fuck unity.
        double delay = 0.5d;

        musicSource.PlayScheduled(AudioSettings.dspTime + delay);
        musicSourceSecondary.PlayScheduled(AudioSettings.dspTime + duration + delay);
    }

    // <summary>
    // Set mixer volume for a given group using 0.0~1.0.
    // </summary>
    public void SetGroupVolume(float volumeNormalized, AudioMixer.Groups group)
    {
        normalizedVolumeValues[group] = volumeNormalized;

        // converting from 0.0f~1.0f to logarhithmic dB scale
        float realVolume = Mathf.Log10(volumeNormalized) * volumeMultiplier;
        audioMixer.SetFloat(group.ToString(), realVolume);
    }

    // <summary>
    // Get mixer volume for a given group in decibells.
    // </summary>
    public float GetGroupVolumeReal(AudioMixer.Groups group)
    {
        audioMixer.GetFloat(group.ToString(), out float volume);
        return volume;
    }

    // <summary>
    // Destroy() all currently looping sound effects.
    // </summary>
    public void DestroyLoopingSources()
    {
        foreach (var source in LoopingAudioSources) Destroy(source);
    }

    // <summary>
    // Get mixer volume for a given group in 0.0~1.0 float.
    // </summary>
    public float GetGroupVolumeNormalized(AudioMixer.Groups group) => normalizedVolumeValues[group];
}