using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : _MenuButton
{
    void Start()
    {
        // TODO: which scene
        button.onClick.AddListener(() => SceneManager.LoadScene("GameScene"));
    }
}