using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class CustomerAI : MonoBehaviour
{
    public int seatId;
    public Transform targetSeat; // Assigned dynamically when customer spawns
    public Transform exitDoor; // Reference to the door transform
    public float stopDistance = 1.5f; // Distance from the seat to stop
    public float waitTime = 15f; // Time before leaving
    public bool isSeated = false;
    public bool isOrderFulfilled = false;
    public UIDocument GameGUI;
    public GameObject quizGeneratorObject;
    private QuizGenerator quizGenerator;   
    private Order orderDetails; 
    private VisualElement orderListContainer;
    private VisualElement orderCardWrapper;
    private int orderValue;
    private NavMeshAgent agent;
    private float timer;
    private Animator animator; // TODO: Needs fix
    private float initialDelay; // New variable for random delay
    private bool hasStartedMoving = false; // New variable to track if customer has started moving
    private float maxWaitTime = 120f; // Maximum time customer will wait
    private float currentWaitTime; // Current remaining time
    private bool isTimerStarted = false;
    private CustomerSpawner spawner;

    void Start()
    {
        Debug.Log("[CustomerAI] Start() called.");

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (quizGenerator == null) {
            if (quizGeneratorObject == null)
            {
                Debug.LogError("[CustomerAI] QuizGenerator not assigned. Will try finding it in scene");
                quizGeneratorObject = GameObject.Find("QuizGenerator");
            }
            quizGenerator = quizGeneratorObject.GetComponent<QuizGenerator>();
            if (quizGenerator == null)
            {
                Debug.LogError("[CustomerAI] QuizGenerator still not found, orderDetails cannot be created.");
                UnityEditor.EditorApplication.isPlaying = false; // Commented out for debugging
                return;
            }
        }

        Debug.Log("[CustomerAI] quizGenerator is not null: " + (quizGenerator != null));

        VisualTreeAsset orderCardTemplate = Resources.Load<VisualTreeAsset>("UI/GameUI/OrderCard");
        if (orderCardTemplate == null)
        {
            Debug.LogError("[CustomerAI] OrderCard template not found");
            UnityEditor.EditorApplication.isPlaying = false; // Commented out for debugging
            return;
        }
        else {
            orderCardWrapper = orderCardTemplate.Instantiate();
            Debug.Log("[CustomerAI] orderCardWrapper instantiated successfully.");
        }

        if (GameGUI == null)
        {
            Debug.LogError("[CustomerAI] GameGUI (UIDocument) not assigned.");
            // UnityEditor.EditorApplication.isPlaying = false; // Commented out for debugging
            return;
        }

        orderListContainer = GameGUI.rootVisualElement.Q<VisualElement>("OrderGroups");
        if (orderListContainer == null)
        {
            Debug.LogError("[CustomerAI] OrderListContainer not found in GameGUI!");
            // UnityEditor.EditorApplication.isPlaying = false; // Commented out for debugging
            return;
        }

        // Set random delay between 1-30 seconds
        initialDelay = Random.Range(1f, 30f);
        Debug.Log("[CustomerAI] initialDelay set to: " + initialDelay);
        agent.isStopped = true; // Stop agent initially
        currentWaitTime = maxWaitTime;

        spawner = FindObjectOfType<CustomerSpawner>();
    }

    void Update()
    {
        if (!hasStartedMoving)
        {
            initialDelay -= Time.deltaTime;
            if (initialDelay <= 0)
            {
                Debug.Log("[CustomerAI] Starting movement towards seat.");
                hasStartedMoving = true;
                agent.isStopped = false;
                Vector3 offsetPosition = CalculateOffsetPosition(targetSeat.position, stopDistance);
                agent.SetDestination(offsetPosition);
                animator.SetBool("Jump", true);
            }
            return;
        }

        // Check if customer has reached their offset seat position
        if (!isSeated && Vector3.Distance(transform.position, agent.destination) < 0.5f)
        {
            Debug.Log("[CustomerAI] Customer reached seat destination.");
            isSeated = true;
            agent.isStopped = true; // Stop movement
            timer = waitTime;

            // Trigger idle animation and display order
            animator.SetBool("Jump", false);
            animator.SetBool("Idle", true);
            
            // Create and display order after setting isSeated
            DisplayOrder();
        }

        // Move the timer logic here, after potential order creation
        if (isSeated && !isOrderFulfilled && orderDetails != null)
        {
            if (!isTimerStarted)
            {
                isTimerStarted = true;
                currentWaitTime = maxWaitTime;
            }

            currentWaitTime -= Time.deltaTime;
            orderDetails.UpdateTime(Time.deltaTime);

            if (currentWaitTime <= 0f)
            {
                Debug.Log("[CustomerAI] Timer ran out, leaving angrily.");
                LeaveRestaurant(false);
            }
        }
    }

    // Calculate an offset position to stop a certain distance from the seat
    private Vector3 CalculateOffsetPosition(Vector3 seatPosition, float distance)
    {
        Vector3 direction = (seatPosition - transform.position).normalized;
        return seatPosition - direction * distance;
    }

    private void DisplayOrder()
    {
        Debug.Log("[CustomerAI] DisplayOrder() called.");
        
        // Create order if it doesn't exist
        if (orderDetails == null) 
        {
            Debug.Log("[CustomerAI] Creating new order...");
            orderValue = quizGenerator.GenerateQuestion();
            orderDetails = new Order
            {
                orderNum = orderValue,
                timeLimit = 15,
                tableNum = seatId,
            };
        }

        Debug.Log("[CustomerAI] Generating order card UI.");
        orderCardWrapper.Q<Label>("OrderLabel").text = orderValue.ToString();
        orderCardWrapper.Q<Label>("TableNumLabel").text = seatId.ToString();
        orderDetails.orderContainer = orderCardWrapper;
        orderListContainer.Add(orderCardWrapper);
        Debug.Log("[CustomerAI] Order card displayed successfully.");
    }

    public void FulfillOrder(int number)
    {
        if (isOrderFulfilled) return;
        if (number != orderValue) {
            Debug.Log("[CustomerAI] Wrong order. Expected: " + orderValue + ", got: " + number);
            return;
        }
        isOrderFulfilled = true;
        Debug.Log("[CustomerAI] Order fulfilled. Leaving happily.");
        LeaveRestaurant(true); // Leave happily
    }

    private void LeaveRestaurant(bool happy)
    {
        Debug.Log("[CustomerAI] LeaveRestaurant called. Happy: " + happy);
        
        // Remove order UI
        if (orderDetails != null && orderDetails.orderContainer != null)
        {
            if (orderListContainer != null)
            {
                orderListContainer.Remove(orderDetails.orderContainer);
            }
            orderDetails = null;
        }

        // Set animation states
        animator.SetBool("Idle", false);
        animator.SetBool("Jump", true);  // Use jump animation for movement
        
        Leave();  // This will trigger the movement to door
    }

    public void Leave()
    {
        if (isSeated)
        {
            Debug.Log("[CustomerAI] Customer leaving through the door.");
            isSeated = false;
            agent.isStopped = false;
            Vector3 doorPosition = exitDoor.position;
            Vector3 offsetPosition = CalculateOffsetPosition(doorPosition, stopDistance);
            agent.SetDestination(offsetPosition);
            StartCoroutine(CheckReachedDoor());
        }
    }

    private IEnumerator CheckReachedDoor()
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, agent.destination) < 0.5f)
            {
                animator.SetBool("Jump", false);
                animator.SetBool("Idle", true);
                
                // Add small delay before destroying
                yield return new WaitForSeconds(0.5f);
                Destroy(gameObject);
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.CustomerLeft(seatId);
        }
    }
}
