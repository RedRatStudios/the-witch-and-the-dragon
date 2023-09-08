using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public int echoesBought = 0;
    public int plasmidsBought = 0;
    public int rerollsBought = 0;
    public int highlightsBought = 0;

    public float marbleMultiplier;
    public float costCreep;

    public float cookingTimeMultiplier = 1f;
    public float monocoinMultplier = 2f;

    public bool surviveBan = false;
    public bool highlithedMessage = false;

    private float timePlaying = 0f;

    private void Awake()
    {
        Instance = this;

        marbleMultiplier = PlayerPrefs.GetFloat("marbleMultiplier", 1f);
        costCreep = PlayerPrefs.GetFloat("costCreep", 1.3f);
    }

    private void Start()
    {
        // TODO: display at the end
        timePlaying += Time.deltaTime;
    }
}
