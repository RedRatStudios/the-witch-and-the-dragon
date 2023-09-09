using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public float cookingTimeMultiplier = 1f;


    // The multiplier that grows with subs
    public float monocoinMultplier = 1f;

    // The multiplier that grows with plasmids
    public float monocoinSmallMultiplier = 1f;

    // Multiplier that grows with murbles
    public float marbleMultiplier;

    // Amount to exponentially increase cost of upgrades by
    public float costCreep;

    public int timesBanned;

    // Multiplier that grows with echoes
    public float AngerMultiplier = 1f;

    public bool surviveBan = false;
    public bool highlithedMessage = false;

    private float timePlaying = 0f;

    private void Awake()
    {
        Instance = this;

        marbleMultiplier = PlayerPrefs.GetFloat("marbleMultiplier", 1f);
        costCreep = PlayerPrefs.GetFloat("costCreep", 1.3f);
        timesBanned = PlayerPrefs.GetInt("timesBanned", 0);
    }

    private void Start()
    {
        // TODO: display at the end
        timePlaying += Time.deltaTime;

        AngerManager.OnMaxAnger += () =>
        {
            timesBanned++;
            PlayerPrefs.SetInt("timesBanned", timesBanned);
            PlayerPrefs.Save();
        };

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenuScene");
            SceneMoodManager.Instance.ChangeMood(SceneMoods.Default);
        }
    }
}
