using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseManager : MonoBehaviour
{
    private VisualElement PauseMenu;
    private Button PauseButton;
    private Button ResumeButton;
    private Button RestartButton;
    private bool isPaused = false;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        PauseMenu = root.Q<VisualElement>("Pause");
        PauseButton = root.Q<Button>("MenuBtn");
        if (PauseMenu == null || PauseButton == null) {
            Debug.LogError("[PauseManager] PauseMenu or PauseButton not found! Please check GameGUI");
            UnityEditor.EditorApplication.isPlaying = false;
        }
        PauseButton.RegisterCallback<ClickEvent>((e) => TogglePauseMenu());

        ResumeButton = root.Q<Button>("ResumeBtn");
        ResumeButton.RegisterCallback<ClickEvent>((e) => ResumeGame());

        RestartButton = root.Q<Button>("RestartBtn");
        RestartButton.RegisterCallback<ClickEvent>((e) => RestartCurrentLevel());

        Time.timeScale = 1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("[PAUSED] Pressed ESC");
            TogglePauseMenu();
        }

        if (isPaused && Input.GetKeyDown(KeyCode.R))
        {
            RestartCurrentLevel();
        }
    }

    public void PauseGame()
    {
        Debug.Log("[PAUSE] Game Paused");
        isPaused = true;
        Time.timeScale = 0;
        PauseMenu.RemoveFromClassList("hide-top");
    }

    public void RestartLevel(int idx)
    {
        SceneManager.LoadScene(idx);
        ResumeGame();
    }

    public void RestartCurrentLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        ResumeGame();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        PauseMenu.AddToClassList("hide-top");
    }

    public void TogglePauseMenu()
    {
        Debug.Log("[PAUSE] Toggling Pause Menu");
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
}
