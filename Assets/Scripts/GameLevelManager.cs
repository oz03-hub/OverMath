using System.Collections.Generic;
using UnityEngine;
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
    }

    void Start()
    {
        timeRemaining = timeLimit;
        lastUpdateTime = Time.time;
        ingredientText.text = "";
        UpdateUI();
    }

    void Update()
    {
        if (gameOver) return;

        //Debug.Log($"[GameLevelManager] Current playerPoints: {playerPoints}");

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
            scoreText.text = playerPoints.ToString();
            //Debug.Log($"[GameLevelManager] ScoreText updated to {playerPoints}");
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
    }

    void GameOver()
    {
        gameOver = true;
        Debug.Log("Game Over!");
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
}
