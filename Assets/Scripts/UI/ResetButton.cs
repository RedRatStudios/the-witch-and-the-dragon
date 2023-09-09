using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetButton : _MenuButton
{
    [SerializeField] private TextMeshProUGUI text;

    void Start()
    {
        State state = State.Once;

        // TODO: which scene
        button.onClick.AddListener(() =>
        {
            switch (state)
            {
                case State.Once:
                    text.text = "Proceed with caution.";
                    state = State.Twice;
                    break;
                case State.Twice:
                    text.text = "All of your apes, gone.";
                    PlayerPrefs.DeleteAll();
                    state = State.Once;
                    break;
            }
        });
    }
}

public enum State
{
    Once, Twice
}