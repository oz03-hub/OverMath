using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    private List<CustomerAI> activeCustomers = new List<CustomerAI>();
    public int score = 0;

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
        foreach (var customer in activeCustomers)
        {
            if (customer == null)
            {
                Debug.LogWarning("[CustomerManager] Skipping null customer in activeCustomers list.");
                continue;
            }

            if (customer.IsInteractable) // Interaction only if the customer is within the trigger zone
            {
                Debug.Log($"[CustomerManager] Interacting with customer at seat {customer.seatId}");

                if (customer.isSeated && !customer.isOrderFulfilled)
                {
                    int playerHoldingNumber = GetPlayerHoldingNumber(); // Retrieve what the player is holding

                    if (playerHoldingNumber == customer.orderValue)
                    {
                        Debug.Log("[CustomerManager] Correct order! Updating score and fulfilling order.");
                        score++;
                        customer.FulfillOrder(playerHoldingNumber);
                    }
                    else
                    {
                        Debug.Log("[CustomerManager] Wrong order! Customer leaving angrily.");
                        customer.LeaveRestaurant(false);
                    }
                }
                else
                {
                    Debug.Log("[CustomerManager] Customer is not ready for interaction (either not seated or already fulfilled).");
                }

                return; // Interact with only one customer per key press
            }
        }

        Debug.Log("[CustomerManager] No interactable customer to interact with.");
    }

    private int GetPlayerHoldingNumber()
    {
        // Implement logic to retrieve the player's currently held item or number
        // For now, return a placeholder value
        Debug.Log("[CustomerManager] Player holding number: Placeholder value");
        return 0; // Replace with actual logic
    }
}
