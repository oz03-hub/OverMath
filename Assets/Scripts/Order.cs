using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Order
{
    public int orderNum;
    public int tableNum;
    public float timeLimit;
    public VisualElement orderContainer;

    public void StartTime() {
        if (orderContainer == null) {
            Debug.LogError("OrderContainer is null!");
            UnityEditor.EditorApplication.isPlaying = false; // For debugging
        }
        var timerElement = orderContainer.Q<VisualElement>("Timer");
        var timerBar = timerElement.Q<VisualElement>("TimerBar");
        if (timerBar != null)
        {
            timerBar.style.width = Length.Percent(100);
        } else {
            Debug.LogError("TimerBar is null!");
            UnityEditor.EditorApplication.isPlaying = false; // For debugging
        }
    }

    public void UpdateTime(float timer)
    {
        var timerElement = orderContainer.Q<VisualElement>("Timer");
        var timerBar = timerElement.Q<VisualElement>("TimerBar");
        if (timerBar != null)
        {
            // Debug.Log("Timer: " + timeRemaining);
            float timeRemainPercent = timer / (float)timeLimit * 100;
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