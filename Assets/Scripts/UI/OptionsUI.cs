using UnityEngine;
using UnityEngine.UIElements;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    private Button backButton;
    private VisualElement root;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        backButton = root.Q<Button>("Back");

        backButton.clicked += () =>
        {
            gameObject.SetActive(false);
            MainMenuUI.Instance.gameObject.SetActive(true);
        };
    }
}