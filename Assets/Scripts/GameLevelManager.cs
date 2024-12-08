using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameLevelManager : MonoBehaviour
{
    public int playerPoints = 0;
    public int pointsToWin = 100;
    public int ordersCompleted = 0;
    public int totalOrders = 5;
    public float timeLimit = 300f;
    public bool hasWon = false;
    public bool gameOver = false;
    public UIDocument GameGUI;
    public List<Order> orderList; //? Should this be here?

    private float timeRemaining;
    private float lastUpdateTime;
    private Label timerText;
    private Label scoreText;
    private Label ingredientText;

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

        // orderList = new List<Order>();
        ingredientText.text = "";
    }

    void Update()
    {
        if (gameOver) return;

        if (Time.time - lastUpdateTime >= 1f)
        {
            lastUpdateTime = Time.time;
            timeRemaining = Mathf.Max(0, timeRemaining - 1f);
            // foreach (Order order in orderList)
            // {
            //     order.UpdateTime();
            //     if (order.timeRemaining == 0)
            //     {
                    
            //         orderList.Remove(order);
            //         order.orderContainer.RemoveFromHierarchy();
            //         break;
            //     }
            // }
        }

        if (timeRemaining <= 0)
        {
            GameOver();
        }

        if (playerPoints >= pointsToWin)
        {
            WinGame();
        }

        if (ordersCompleted >= totalOrders)
        {
            CompleteAllTasks();
        }

        UpdateUI();
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
        if (!gameOver && ordersCompleted < totalOrders)
        {
            ordersCompleted++;
        }
    }

    public void UpdateIngredientText(string ingredient)
    {
        if (string.IsNullOrEmpty(ingredient))
        {
            ingredientText.text = "";
        } else {
            ingredientText.text = ingredient;
        }
    }
}
