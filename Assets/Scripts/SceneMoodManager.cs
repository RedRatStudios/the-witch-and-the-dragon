using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum SceneMoods
{
    Default, Persona4, Undertale
}

public class SceneMoodManager : MonoBehaviour
{
    public static SceneMoodManager Instance;
    public SceneMoods mood = new();

    public static event Action<SceneMoods> OnMoodUpdate;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        mood = SceneMoods.Default;
        AngerManager.OnMaxAnger += () => StartCoroutine(SwitchAfterSec());

        if (Input.GetKeyDown(KeyCode.Q))
            ChangeMood(RandomEnumValue<SceneMoods>());
    }

    // HACK: don't talk to me.
    // IEnumerator WaitForAnFrame()
    // {
    //     yield return new WaitForSeconds(.1f);
    //     // I'm going to invoke a racial slur if this bithc doesn't work
    //     OnMoodUpdate?.Invoke(mood);
    // }

    IEnumerator SwitchAfterSec(float seconds = 5)
    {
        yield return new WaitForSeconds(seconds);
        ChangeMood(RandomEnumValue<SceneMoods>());
    }

    public void ChangeMood(SceneMoods mood)
    {
        // if (this.mood == mood) return;

        Debug.Log("Switched to " + mood.ToString());
        this.mood = mood;
        OnMoodUpdate?.Invoke(mood);
    }

    // shamelessely stolen
    static System.Random _R = new();
    static T RandomEnumValue<T>()
    {
        var v = Enum.GetValues(typeof(T));
        return (T)v.GetValue(_R.Next(v.Length));
    }
}
