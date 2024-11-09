using UnityEngine;

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

    void Start()
    {
        timeRemaining = timeLimit;
    }

    void Update()
    {
        if (gameOver) return;

        timeRemaining -= Time.deltaTime;

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
