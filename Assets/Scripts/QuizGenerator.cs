using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class QuizGenerator : MonoBehaviour
{
    public int[] allowedNumbers;
    public Operators[] allowedOperators;
    public int maxCombinationLength = 5;
    public enum Operators
    {
        Addition,
        Subtraction,
        Multiplication
    }

    private Dictionary<Operators, Func<int, int, int>> operatorFunctions;
    private List<int> orderList;
    void Awake()
    {
        InitializeOperatorFunctions();
    }

    void Start()
    {
        if (allowedNumbers == null || allowedNumbers.Length == 0)
        {
            Debug.LogError("[QuizGenerator] allowedNumbers is not set or is empty!");
            return; // Prevent running GenerateQuestion if we have no allowed numbers
        }

        if (allowedOperators == null || allowedOperators.Length == 0)
        {
            Debug.LogError("[QuizGenerator] allowedOperators is not set or is empty!");
            return; // Prevent running GenerateQuestion if we have no allowed operators
        }

        orderList = new List<int>();
    }


    private void InitializeOperatorFunctions()
    {
        operatorFunctions = new Dictionary<Operators, Func<int, int, int>>
        {
            { Operators.Addition, (a, b) => a + b },
            { Operators.Subtraction, (a, b) => a - b },
            { Operators.Multiplication, (a, b) => a * b }
        };
    }

    private int PerformOperation(Operators op, int a, int b)
    {
        if (operatorFunctions.TryGetValue(op, out Func<int, int, int> func))
        {
            return func(a, b);
        }
        throw new ArgumentException("Invalid operator");
    }

    public int GenerateQuestion()
    {
        if (allowedNumbers == null || allowedNumbers.Length == 0)
        {
            Debug.LogError("[QuizGenerator] Cannot generate question, allowedNumbers is empty or null!");
            return 0;
        }

        if (allowedOperators == null || allowedOperators.Length == 0)
        {
            Debug.LogError("[QuizGenerator] Cannot generate question, allowedOperators is empty or null!");
            return 0;
        }

        if (maxCombinationLength <= 0)
        {
            Debug.LogError("[QuizGenerator] maxCombinationLength must be greater than 0!");
            return 0;
        }

        int a = 0;
        int b = 0;
        Operators op = Operators.Addition;
        int length = UnityEngine.Random.Range(1, maxCombinationLength);
        for (int i = 0; i < length; i++)
        {
            b = allowedNumbers[UnityEngine.Random.Range(0, allowedNumbers.Length)];
            op = allowedOperators[UnityEngine.Random.Range(0, allowedOperators.Length)];

            // For the first iteration, set 'a' before the loop continues.
            if (i == 0)
            {
                a = allowedNumbers[UnityEngine.Random.Range(0, allowedNumbers.Length)];
            }

            a = PerformOperation(op, a, b);
        }

        return a;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
