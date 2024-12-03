using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CustomerAI : MonoBehaviour
{
    public Transform targetSeat; // Assigned dynamically when customer spawns
    public Transform exitDoor; // Reference to the door transform
    public float stopDistance = 1.5f; // Distance from the seat to stop
    public float waitTime = 15f; // Time before leaving
    public GameObject orderUI; // UI element showing order above customer

    private NavMeshAgent agent;
    private float timer;
    private float initialDelay; // New variable for random delay
    private bool hasStartedMoving = false; // New variable to track if customer has started moving
    public bool isSeated = false;
    public bool isOrderFulfilled = false;
    private Animator animator;
    
    private int orderValue;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        // Set random delay between 2-10 seconds
        initialDelay = Random.Range(1f, 30f);
        agent.isStopped = true; // Stop agent initially
    }

    void Update()
    {
        if (!hasStartedMoving)
        {
            initialDelay -= Time.deltaTime;
            if (initialDelay <= 0)
            {
                hasStartedMoving = true;
                agent.isStopped = false;
                // Set destination with an offset to stop at a distance
                Vector3 offsetPosition = CalculateOffsetPosition(targetSeat.position, stopDistance);
                agent.SetDestination(offsetPosition);
            }
            return;
        }

        // Check if customer has reached their offset seat position
        if (!isSeated && Vector3.Distance(transform.position, agent.destination) < 0.5f)
        {
            isSeated = true;
            agent.isStopped = true; // Stop movement
            timer = waitTime;

            // Trigger idle animation and display order
            animator.SetBool("Idle", true);
            DisplayOrder();
        }

        // Countdown timer if seated and order isn't fulfilled
        if (isSeated && !isOrderFulfilled)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                LeaveRestaurant(false); // Leave angrily if time runs out
            }
        }
    }

    // Calculate an offset position to stop a certain distance from the seat
    private Vector3 CalculateOffsetPosition(Vector3 seatPosition, float distance)
    {
        Vector3 direction = (seatPosition - transform.position).normalized;
        return seatPosition - direction * distance;
    }

    // Display the customer's order
    private void DisplayOrder()
    {
        orderUI.SetActive(true);
        orderValue = Random.Range(5, 20); // Example: Random value between 5 and 20
        orderUI.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = orderValue.ToString(); // Show order value
    }

    // Call when the player's equation matches the customer's order
    public void FulfillOrder()
    {
        if (isOrderFulfilled) return; // Avoid duplicate fulfillment

        isOrderFulfilled = true;
        orderUI.SetActive(false); // Hide order UI
        LeaveRestaurant(true); // Leave happily
    }

    // Handle the customer's departure
    private void LeaveRestaurant(bool happy)
    {
        if (happy)
        {
            animator.SetTrigger("Happy"); // Trigger happy animation
        }
        else
        {
            animator.SetTrigger("Angry"); // Trigger angry animation
        }

        Destroy(gameObject, 2f); // Remove customer after animations play
    }

    // Public function to make customer leave through the door
    public void Leave()
    {
        if (isSeated)
        {
            isSeated = false;
            agent.isStopped = false;
            orderUI.SetActive(false);
            
            // Calculate position near the door
            Vector3 doorPosition = exitDoor.position;
            Vector3 offsetPosition = CalculateOffsetPosition(doorPosition, stopDistance);
            
            // Set new destination
            agent.SetDestination(offsetPosition);
            
            // Start coroutine to check when customer reaches door
            StartCoroutine(CheckReachedDoor());
        }
    }

    private IEnumerator CheckReachedDoor()
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, agent.destination) < 0.5f)
            {
                // Play walking animation if you have one
                animator.SetBool("Idle", false);
                
                // Destroy the customer object
                Destroy(gameObject);
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
