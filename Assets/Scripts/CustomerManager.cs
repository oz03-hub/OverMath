using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public Transform player;
    private List<CustomerAI> activeCustomers = new List<CustomerAI>();

    public int pointsForCorrectOrder = 10;
    public int pointsForIncorrectOrder = 5;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            InteractWithCustomer();
        }
    }

    public void RegisterCustomer(CustomerAI customer)
    {
        if (customer == null)
        {
            Debug.LogWarning("[CustomerManager] Attempted to register a null customer.");
            return;
        }

        if (!activeCustomers.Contains(customer))
        {
            activeCustomers.Add(customer);
            Debug.Log($"[CustomerManager] Registered customer at seat {customer.seatId}");
        }
    }

    public void DeregisterCustomer(CustomerAI customer)
    {
        if (activeCustomers.Contains(customer))
        {
            activeCustomers.Remove(customer);
            Debug.Log($"[CustomerManager] Deregistered customer at seat {customer.seatId}");
        }
    }

    private void InteractWithCustomer()
    {
        CustomerAI closestCustomer = null;
        float closestDistance = float.MaxValue;

        foreach (var customer in activeCustomers)
        {
            if (customer == null || !customer.IsInteractable)
                continue;

            float distanceToPlayer = Vector3.Distance(player.position, customer.transform.position);
            if (distanceToPlayer < closestDistance)
            {
                closestDistance = distanceToPlayer;
                closestCustomer = customer;
            }
        }

        if (closestCustomer != null)
        {
            HandleCustomerInteraction(closestCustomer);
        }
        else
        {
            Debug.Log("[CustomerManager] No interactable customer nearby.");
        }
    }

    private void HandleCustomerInteraction(CustomerAI customer)
    {
        if (customer.isSeated && !customer.isOrderFulfilled)
        {
            int playerHoldingNumber = GetPlayerHoldingNumber(customer);
            Debug.Log($"[CustomerManager] Player attempting to fulfill order with number: {playerHoldingNumber}");

            GameLevelManager levelManager = FindObjectOfType<GameLevelManager>();

            if (levelManager == null)
            {
                Debug.LogError("[CustomerManager] GameLevelManager not found in scene!");
                return;
            }

            if (playerHoldingNumber == customer.orderValue)
            {
                Debug.Log("[CustomerManager] Correct order! Adding points.");
                levelManager.AddPoints(pointsForCorrectOrder);
                customer.FulfillOrder(playerHoldingNumber);
            }
            else
            {
                Debug.Log("[CustomerManager] Incorrect order! Subtracting points.");
                levelManager.SubtractPoints(pointsForIncorrectOrder);
                customer.LeaveRestaurant(false);
            }
            levelManager.UpdateIngredientText("");
        }
        else
        {
            Debug.Log("[CustomerManager] Customer is not ready for interaction.");
            GameLevelManager levelManager = FindObjectOfType<GameLevelManager>();
            if (levelManager != null)
            {
                levelManager.UpdateIngredientText("");
            }
        }
    }


    private int GetPlayerHoldingNumber(CustomerAI customer)
    {
        GameLevelManager gameManager = FindObjectOfType<GameLevelManager>();
        if (gameManager == null)
        {
            Debug.LogError("[CustomerManager] GameLevelManager not found in the scene!");
            return -1;
        }

        if (gameManager.ingredientText == null)
        {
            Debug.LogError("[CustomerManager] IngredientText is null in GameLevelManager!");
            return -1;
        }

        string ingredientValue = gameManager.ingredientText.text;
        Debug.Log($"[CustomerManager] Player is holding ingredient: {ingredientValue}");

        if (int.TryParse(ingredientValue, out int result))
        {
            Debug.Log($"[CustomerManager] Parsed ingredient value as: {result}");
            return result;
        }
        else
        {
            Debug.LogWarning("[CustomerManager] IngredientText value could not be parsed to an integer!");
            return -1;
        }
    }
}
