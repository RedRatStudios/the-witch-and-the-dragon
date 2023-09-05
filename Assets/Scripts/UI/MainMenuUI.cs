using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance { get; private set; }

    private VisualElement root;

    private VisualElement menuContainer;
    private VisualElement optionsContainer;

    private Button startButton;
    private Button optionsButton;
    private Button backButton;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        menuContainer = root.Q<VisualElement>("MenuContainer");
        optionsContainer = root.Q<VisualElement>("OptionsContainer");

        startButton = root.Q<Button>("Start");
        optionsButton = root.Q<Button>("Options");
        backButton = root.Q<Button>("Back");

        startButton.clicked += () =>
        {
            SceneManager.LoadScene(Common.Scenes.GameScene.ToString());
        };

        optionsButton.clicked += () =>
        {
            menuContainer.visible = false;
            optionsContainer.visible = true;
            backButton.visible = true;
        };

        backButton.clicked += () =>
        {
            menuContainer.visible = true;
            optionsContainer.visible = false;
            backButton.visible = false;
        };
    }
}
