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

        Debug.Log("[CustomerAI] Generating orderValue...");
        orderValue = quizGenerator.GenerateQuestion();
        Debug.Log("[CustomerAI] OrderValue generated: " + orderValue);

        orderDetails = new Order
        {
            orderNum = orderValue,
            timeLimit = 15,
            tableNum = seatId,
        };
        Debug.Log("[CustomerAI] OrderDetails created: " + (orderDetails != null));

        // Set random delay between 1-30 seconds
        initialDelay = Random.Range(1f, 30f);
        Debug.Log("[CustomerAI] initialDelay set to: " + initialDelay);
        agent.isStopped = true; // Stop agent initially
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
            DisplayOrder();
            isSeated = true;
            agent.isStopped = true; // Stop movement
            timer = waitTime;

            // Trigger idle animation and display order
            animator.SetBool("Jump", false);
            animator.SetBool("Idle", true);
        }

        if (isSeated && orderDetails == null) {
            Debug.LogError("[CustomerAI] Order details not found at seating time!");
            UnityEditor.EditorApplication.isPlaying = false; // Commented out for debugging
        }

        // Countdown timer if seated and order isn't fulfilled
        if (isSeated && !isOrderFulfilled && orderDetails != null)
        {
            timer -= Time.deltaTime;
            orderDetails.UpdateTime(Time.deltaTime);

            if (timer <= 0f)
            {
                Debug.Log("[CustomerAI] Timer ran out, leaving angrily.");
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

    private void DisplayOrder()
    {
        Debug.Log("[CustomerAI] DisplayOrder() called.");
        if (this.orderDetails == null) {
            Debug.LogError("[CustomerAI] Order details not found in DisplayOrder!");
            UnityEditor.EditorApplication.isPlaying = false; // Commented out for debugging
            return;
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
        if (happy)
        {
            // animator.SetTrigger("Happy"); // Trigger happy animation
        }
        else
        {
            // animator.SetTrigger("Angry"); // Trigger angry animation
        }
        orderCardWrapper.RemoveFromHierarchy();
        Destroy(gameObject, 2f); // Remove customer after animations play
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
                animator.SetBool("Idle", false);
                Destroy(gameObject);
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
