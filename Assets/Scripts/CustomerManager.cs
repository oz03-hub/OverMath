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
            // Pass the customer object to GetPlayerHoldingNumber
            int playerHoldingNumber = GetPlayerHoldingNumber(customer);

            Debug.Log($"[CustomerManager] Player attempting to fulfill order with number: {playerHoldingNumber}");

            if (playerHoldingNumber == customer.orderValue)
            {
                Debug.Log("[CustomerManager] Correct order! Updating score and fulfilling order.");
                score++;
                customer.FulfillOrder(playerHoldingNumber);
            }
            else
            {
                Debug.Log("[CustomerManager] Incorrect order! Customer leaving angrily.");
                customer.LeaveRestaurant(false);
            }
        }
        else
        {
            Debug.Log("[CustomerManager] Customer is not ready for interaction (either not seated or already fulfilled).");
        }
    }


    private int GetPlayerHoldingNumber(CustomerAI customer)
    {
        if (customer != null)
        {
            int customerOrder = customer.orderValue; // Fetch the customer's order value
            Debug.Log($"[CustomerManager] Player is holding number: {customerOrder}");
            return customerOrder;
        }

        Debug.LogWarning("[CustomerManager] No valid customer passed to GetPlayerHoldingNumber.");
        return -1; // Return -1 if no valid order is retrieved
    }

}
