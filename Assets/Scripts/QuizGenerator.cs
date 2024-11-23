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
    void Start()
    {
        InitializeOperatorFunctions();
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

    private int GenerateQuestion()
    {
        int a = 0;
        for (int i = 0; i < UnityEngine.Random.Range(1, maxCombinationLength); i++)
        {
            int b = allowedNumbers[UnityEngine.Random.Range(0, allowedNumbers.Length)];
            Operators op = allowedOperators[UnityEngine.Random.Range(0, allowedOperators.Length)];
            if (i == 0)
            {
                a = allowedNumbers[UnityEngine.Random.Range(0, allowedNumbers.Length)];
            }
            a = PerformOperation(op, a, b);
            if (a < 0) a = 0; // Ensure non-negative result
        }

        return a;
    }

    public void InitOrder()
    {
        StartCoroutine(AwaitQuiz());
    }

    public List<int> GetOrder()
    {
        return orderList;
    }

    IEnumerator AwaitQuiz()
    {
        while (true)
        {
            int result = GenerateQuestion();
            orderList.Add(result);
            yield return new WaitForSeconds(10.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
