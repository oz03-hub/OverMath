using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameLevelManager : MonoBehaviour
{
    public int playerPoints = 0;
    public int pointsToWin = 100;
    public int tasksCompleted = 0;
    public int totalTasks = 5;
    public float timeLimit = 300f;
    public bool hasWon = false;
    public bool gameOver = false;

    private float timeRemaining;
    private float lastUpdateTime;

    public TMP_Text timerText;
    public TMP_Text scoreText;
    public TMP_Text equationText;
    public TMP_Text feedbackText;

    void Start()
    {
        timeRemaining = timeLimit;
        lastUpdateTime = Time.time;
    }

    void Update()
    {
        if (gameOver) return;

        if (Time.time - lastUpdateTime >= 1f)
        {
            lastUpdateTime = Time.time;
            timeRemaining = Mathf.Max(0, timeRemaining - 1f);
        }

        if (timeRemaining <= 0)
        {
            GameOver();
        }

        if (playerPoints >= pointsToWin)
        {
            WinGame();
        }

        if (tasksCompleted >= totalTasks)
        {
            CompleteAllTasks();
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.Max(0, Mathf.FloorToInt(timeRemaining)).ToString();
        }

        if (scoreText != null)
        {
            scoreText.text = "Score: " + playerPoints.ToString();
        }
    }

    public void ShowEquation(string equation)
    {
        if (equationText != null)
        {
            equationText.text = "Solve the equation: " + equation;
        }
    }

    public void ShowFeedback(bool isCorrect)
    {
        if (feedbackText != null)
        {
            feedbackText.text = isCorrect ? "Success!" : "Try again, so close!";
            feedbackText.color = isCorrect ? Color.green : Color.red;
        }
    }

    void WinGame()
    {
        hasWon = true;
        gameOver = true;
    }

    void GameOver()
    {
        gameOver = true;
    }

    void CompleteAllTasks()
    {
        if (!gameOver)
        {
            Debug.Log("All tasks completed!");
        }
    }

    public void AddPoints(int points)
    {
        if (!gameOver)
        {
            playerPoints += points;
        }
    }

    public void CompleteTask()
    {
        if (!gameOver && tasksCompleted < totalTasks)
        {
            tasksCompleted++;
        }
    }
}
