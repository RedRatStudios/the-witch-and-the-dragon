using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

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
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
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
        AlchemyManager.OnFunnyMessageSent += () => PlaySoundEffect(audioRef.witchPositive);
        AlchemyManager.OnSpicyMessageSent += () => PlaySoundEffect(audioRef.messageCooked_Amazing);

        // too annoying
        // MonocoinManager.OnMonocoinsChanged += () => PlaySoundEffect(audioRef.coin, volume: 0.08f);

        BuyUpgradeButton.OnButtonPressed += () => PlaySoundEffect(audioRef.buttonPress);
        BuyUpgradeButton.OnButtonReleased += () => PlaySoundEffect(audioRef.buttonRelease);

        UpgradeManager.OnRogueModSuccess += () => PlaySoundEffect(audioRef.coin);
        UpgradeManager.OnRogueModSuccess += () => PlaySoundEffect(audioRef.witchPositive);
        UpgradeManager.OnRogueModFail += () => PlaySoundEffect(audioRef.witchNegative);

        AngerManager.OnMaxAnger += () => PlaySoundEffect(audioRef.gettingBanned);

        SceneMoodManager.OnMoodUpdate += mood => { ChooseMusic(mood); };
        SceneManager.activeSceneChanged += (big, boobs) => PlaySoundEffect(audioRef.loop_boil, true, volume: .7f);

        _MenuButton.OnButtonPressed += () => PlaySoundEffect(audioRef.buttonPress);
        _MenuButton.OnButtonReleased += () => PlaySoundEffect(audioRef.buttonRelease);

        _GameButton.OnButtonPressed += () => PlaySoundEffect(audioRef.buttonPress);
        _GameButton.OnButtonReleased += () => PlaySoundEffect(audioRef.buttonRelease);

        PlayMusic(musicRef.mainMenuLoop, delay: 0f, fadeout: false, fadein: false);
    }

    private void ChooseMusic(SceneMoods mood)
    {
        // quick and dirty
        if (mood == SceneMoods.Default)
            PlayMusic(musicRef.mainMenuLoop, volume: 0.8f);
        else if (mood == SceneMoods.Persona4)
            PlayMusic(musicRef.dreamComeTrueLoop, musicRef.dreamComeTrueIntro);
        else if (mood == SceneMoods.Undertale)
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
    public void PlayMusic(AudioClip musicLoop, AudioClip musicIntro = null, float volume = 1f, bool fadeout = true, float delay = 2f, bool fadein = true)
    {

        if (fadeout)
        {
            StartCoroutine(AudioFade.FadeOut(musicSource, delay, Mathf.SmoothStep));
            StartCoroutine(AudioFade.FadeOut(musicSourceSecondary, delay, Mathf.SmoothStep));
        }
        else
        {
            musicSource.Stop();
            musicSourceSecondary.Stop();
        }

        StartCoroutine(PlayAfterDelay(musicLoop, musicIntro, thisdelay: delay + 0.1f, fadein: fadein));
    }

    private IEnumerator PlayAfterDelay(AudioClip musicLoop, AudioClip musicIntro, float thisdelay = .8f, bool fadein = true)
    {
        yield return new WaitForSeconds(thisdelay);

        if (musicIntro == null)
        {
            musicSource.clip = musicLoop;
            musicSource.loop = true;
            musicSource.volume = .5f;

            musicSourceSecondary.clip = null;

            if (musicLoop != null)
                musicSource.Play();
        }
        else
        {

            // Play out the intro if provided, schedule loop to play
            musicSource.clip = musicIntro;
            musicSource.loop = false;
            musicSource.volume = .5f;

            musicSourceSecondary.clip = musicLoop;
            musicSourceSecondary.loop = true;
            musicSourceSecondary.volume = .5f;

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

        // if (fadein)
        //     StartCoroutine(AudioFade.FadeIn(musicSource, 1f, Mathf.SmoothStep));
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

// Shamelessly stolen
public class AudioFade
{
    public static IEnumerator FadeOut(AudioSource source, float fadingTime, Func<float, float, float, float> Interpolate)
    {
        float startVolume = source.volume;
        float frameCount = fadingTime / Time.deltaTime;
        float framesPassed = 0;

        while (framesPassed <= frameCount)
        {
            var t = framesPassed++ / frameCount;
            source.volume = Interpolate(startVolume, 0, t);
            yield return null;
        }

        source.volume = 0.01f;
        source.Stop();
    }

    public static IEnumerator FadeIn(AudioSource source, float fadingTime, Func<float, float, float, float> Interpolate)
    {
        source.volume = 0.1f;

        float resultVolume = 0.5f;
        float frameCount = fadingTime / Time.deltaTime;
        float framesPassed = 0;

        while (framesPassed <= frameCount)
        {
            var t = framesPassed++ / frameCount;
            source.volume = Interpolate(0, resultVolume, t);
            Debug.Log(source.volume);
            yield return null;
        }

        source.volume = resultVolume;
    }
}