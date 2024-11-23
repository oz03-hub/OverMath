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
    public QuizGenerator quizGenerator;

    private float timeRemaining;
    private float lastUpdateTime;

    private Label timerText;
    private Label scoreText;
    private VisualElement orderListContainer;
    private Label ingredientText;

    // Bad code alert pls dont shame me
    private class Order {
        public int orderNum;
        public int tableNum;
        public int timeRemaining;
        public int timeLimit;
        public bool isCompleted;
        public VisualElement orderContainer;

        public void UpdateTime() {
            timeRemaining = Mathf.Max(0, timeRemaining - 1);
            var timerElement = orderContainer.Q<VisualElement>("Timer");
            var timerBar = timerElement.Q<VisualElement>("TimerBar");
            if (timerBar != null)
            {
                Debug.Log("Timer: " + timeRemaining);
                float timeRemainPercent = timeRemaining / (float)timeLimit * 100;
                if (timeRemainPercent < 20)
                {
                    timerBar.style.backgroundColor = new StyleColor(new Color32(225, 112, 85, 255));
                }
                else if (timeRemainPercent < 50)
                {
                    timerBar.style.backgroundColor = new StyleColor(new Color32(253, 203, 110, 255));
                }
                timerBar.style.width = Length.Percent(timeRemainPercent);
            }
        }
    }
    private List<Order> orderList;

    void OnEnable()
    {
        var root = GameGUI.rootVisualElement;
        timerText = root.Q<Label>("Time");
        scoreText = root.Q<Label>("Score");
        orderListContainer = root.Q<VisualElement>("OrderGroups");
        ingredientText = root.Q<Label>("IngredientText");
    }

    void Start()
    {
        timeRemaining = timeLimit;
        lastUpdateTime = Time.time;
        if (quizGenerator == null) {
            Debug.LogError("QuizGenerator not assigned. Will try finding it in scene");
            quizGenerator = FindObjectOfType<QuizGenerator>();
        }
        quizGenerator.InitOrder();
        orderList = new List<Order>();
    }

    void Update()
    {
        if (gameOver) return;

        if (Time.time - lastUpdateTime >= 1f)
        {
            lastUpdateTime = Time.time;
            timeRemaining = Mathf.Max(0, timeRemaining - 1f);
            foreach (Order order in orderList)
            {
                order.UpdateTime();
            }
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

        if (orderList != null) {
            // Add new order to GUI if there is
            Debug.Log("wooooooooooooooooooooooooooooooooooooo");
            List<int> orders = quizGenerator.GetOrder();
            if (orders.Count > orderList.Count && orders.Count < totalOrders)
            {
                int newOrderNum = orders[orders.Count - 1];
                int givenTime = Random.Range(100, 300);
                Order newOrder = new Order
                {
                    orderNum = newOrderNum,
                    tableNum = Random.Range(1, 10), // TODO: Fix later
                    timeRemaining = givenTime,
                    timeLimit = givenTime,
                    isCompleted = false
                };
                Debug.Log("NEW ORDER -----------------");
                Debug.Log(newOrderNum);

                VisualTreeAsset orderCardTemplate = Resources.Load<VisualTreeAsset>("UI/GameUI/OrderCard");
                if (orderCardTemplate == null)
                {
                    Debug.LogError("OrderCard template not found");
                    return;
                }
                VisualElement orderCardWrapper = orderCardTemplate.Instantiate();
  
                var orderLabel = orderCardWrapper.Q<Label>("OrderLabel");
                orderLabel.text = newOrder.orderNum.ToString();

                var tableNumLabel = orderCardWrapper.Q<Label>("TableNumLabel");
                tableNumLabel.text = newOrder.tableNum.ToString();

                newOrder.orderContainer = orderCardWrapper;
                orderList.Add(newOrder);
                orderListContainer.Add(orderCardWrapper);
            }
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
}
