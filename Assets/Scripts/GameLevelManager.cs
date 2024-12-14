using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameLevelManager : MonoBehaviour
{
    public int playerPoints = 0;
    public int pointsToWin = 100;
    public float timeLimit = 300f;
    public bool hasWon = false;
    public bool gameOver = false;
    public UIDocument GameGUI;
    private float timeRemaining;
    private float lastUpdateTime;
    private Label timerText;
    private Label scoreText;
    public Label ingredientText;

    private VisualElement LoseMenu;
    private VisualElement WinMenu;
    private Button RetryButton;
    private Button ContinueButton;


    void OnEnable()
    {
        var root = GameGUI.rootVisualElement;
        timerText = root.Q<Label>("Time");
        scoreText = root.Q<Label>("Score");
        ingredientText = root.Q<Label>("IngredientText");

        if (ingredientText == null)
        {
            Debug.LogError("IngredientText not found in GameGUI!");
        }

        LoseMenu = root.Q<VisualElement>("Lose");
        WinMenu = root.Q<VisualElement>("Win");
        RetryButton = LoseMenu.Q<Button>("RetryBtn");
        ContinueButton = WinMenu.Q<Button>("ContinueBtn");

        RetryButton.RegisterCallback<ClickEvent>((e) => RestartCurrentLevel());
        ContinueButton.RegisterCallback<ClickEvent>((e) => GoToNextLevel());
    }

    void Start()
    {
        timeRemaining = timeLimit;
        lastUpdateTime = Time.time;
        ingredientText.text = "";
        scoreText.text = $"0/{pointsToWin.ToString()}";
        UpdateUI();
    }

    void Update()
    {
        if (gameOver) return;

        if (Time.time - lastUpdateTime >= 1f)
        {
            lastUpdateTime = Time.time;
            timeRemaining = Mathf.Max(0, timeRemaining - 1f);
            UpdateUI();
        }

        if (timeRemaining <= 0)
        {
            GameOver();
        }

        if (playerPoints >= pointsToWin)
        {
            WinGame();
        }
    }

    void OnDisable()
    {
        timerText = null;
        scoreText = null;
        ingredientText = null;
    }

    void UpdateUI()
    {
        if (timerText != null)
        {
            UpdateTime(Mathf.Max(0, Mathf.FloorToInt(timeRemaining)));
        }

        if (scoreText != null)
        {
            scoreText.text = $"{playerPoints.ToString()}/{pointsToWin.ToString()}";
        }
    }

    void UpdateTime(int time)
    {
        int minutes = time / 60;
        int seconds = time % 60;
        string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerText.text = formattedTime;
    }

    void WinGame()
    {
        hasWon = true;
        gameOver = true;
        Debug.Log("You win!");
        Time.timeScale = 0;
        WinMenu.RemoveFromClassList("hide-top");
    }

    void GameOver()
    {
        gameOver = true;
        Debug.Log("Game Over!");
        Time.timeScale = 0;
        LoseMenu.RemoveFromClassList("hide-top");
    }

   public void AddPoints(int points)
    {
        if (!gameOver)
        {
            playerPoints += points;
            Debug.Log($"[GameLevelManager] Points added. New score: {playerPoints}");
            UpdateUI();
        }
    }

    public void SubtractPoints(int points)
    {
        if (!gameOver)
        {
            playerPoints = Mathf.Max(0, playerPoints - points);
            Debug.Log($"[GameLevelManager] Points subtracted. New score: {playerPoints}");
            UpdateUI();
        }
    }

    public void UpdateIngredientText(string ingredient)
    {
        if (string.IsNullOrEmpty(ingredient))
        {
            ingredientText.text = "";
        }
        else
        {
            ingredientText.text = ingredient;
        }
    }

    public void RestartCurrentLevel()
    {
        LoseMenu.AddToClassList("hide-top");
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void GoToNextLevel()
    {
        WinMenu.AddToClassList("hide-top");
        Time.timeScale = 1;
        int nextScene = SceneManager.GetActiveScene().buildIndex+1;
        SceneManager.LoadScene(nextScene);
    }
}
