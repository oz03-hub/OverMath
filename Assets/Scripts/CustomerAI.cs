using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.UIElements;

public class CustomerAI : MonoBehaviour
{
    public int seatId;
    public Transform targetSeat;
    public Transform exitDoor;
    public float stopDistance = 1.5f;
    public float waitTime = 30f;
    public bool isSeated = false;
    public bool isOrderFulfilled = false;
    public bool IsInteractable { get; private set; } = false;
    public UIDocument GameGUI;
    public GameObject quizGeneratorObject;
    private QuizGenerator quizGenerator;
    private Order orderDetails;
    private VisualElement orderListContainer;
    private VisualElement orderCardWrapper;
    public int orderValue;
    private NavMeshAgent agent;
    private float timer;
    private Animator animator;
    private float initialDelay;
    private bool hasStartedMoving = false;
    private bool isTimerStarted = false;
    private CustomerSpawner spawner;
    private bool leaving = false;

    void Start()
    {
        Debug.Log("[CustomerAI] Start() called.");

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (quizGenerator == null)
        {
            if (quizGeneratorObject == null)
            {
                Debug.LogError("[CustomerAI] QuizGenerator not assigned. Will try finding it in scene");
                quizGeneratorObject = GameObject.Find("QuizGenerator");
            }
            quizGenerator = quizGeneratorObject.GetComponent<QuizGenerator>();
            if (quizGenerator == null)
            {
                Debug.LogError("[CustomerAI] QuizGenerator still not found, orderDetails cannot be created.");
                UnityEditor.EditorApplication.isPlaying = false; // For debugging
                return;
            }
        }

        VisualTreeAsset orderCardTemplate = Resources.Load<VisualTreeAsset>("UI/GameUI/OrderCard");
        if (orderCardTemplate == null)
        {
            Debug.LogError("[CustomerAI] OrderCard template not found");
            UnityEditor.EditorApplication.isPlaying = false;
            return;
        }

        orderCardWrapper = orderCardTemplate.Instantiate();
        if (GameGUI == null)
        {
            Debug.LogError("[CustomerAI] GameGUI (UIDocument) not assigned.");
            return;
        }

        orderListContainer = GameGUI.rootVisualElement.Q<VisualElement>("OrderGroups");
        if (orderListContainer == null)
        {
            Debug.LogError("[CustomerAI] OrderListContainer not found in GameGUI!");
            return;
        }

        initialDelay = Random.Range(1f, 10f);
        agent.isStopped = true;

        spawner = FindFirstObjectByType<CustomerSpawner>();

        CustomerManager manager = FindFirstObjectByType<CustomerManager>();
        if (manager != null)
        {
            manager.RegisterCustomer(this);
        }
    }

    void OnDestroy()
    {
        CustomerManager manager = FindFirstObjectByType<CustomerManager>();
        if (manager != null)
        {
            manager.DeregisterCustomer(this);
        }
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
                Vector3 offsetPosition = CalculateOffsetPosition(targetSeat.position, stopDistance);
                agent.SetDestination(offsetPosition);
                animator.SetBool("Jump", true);
            }
            return;
        }

        if (!isSeated && Vector3.Distance(transform.position, agent.destination) < 1.5f)
        {
            isSeated = true;
            agent.isStopped = true;
            timer = waitTime;

            animator.SetBool("Jump", false);
            animator.SetBool("Idle", true);

            if (!leaving)
            {
                DisplayOrder();
            }
        }
    }

    private Vector3 CalculateOffsetPosition(Vector3 seatPosition, float distance)
    {
        Vector3 direction = (seatPosition - transform.position).normalized;
        return seatPosition - direction * distance;
    }

    private void DisplayOrder()
    {
        if (orderDetails == null)
        {
            orderValue = quizGenerator.GenerateQuestion();
            orderDetails = new Order
            {
                orderNum = orderValue,
                timeLimit = waitTime,
                tableNum = seatId,
            };
            StartCoroutine(CountdownTimer());
        }

        orderCardWrapper.Q<Label>("OrderLabel").text = orderValue.ToString();
        orderCardWrapper.Q<Label>("TableNumLabel").text = seatId.ToString();

        orderDetails.orderContainer = orderCardWrapper;
        orderDetails.StartTime();
        orderListContainer.Add(orderCardWrapper);
    }

    private IEnumerator CountdownTimer()
    {
        while (timer > 0)
        {
            yield return new WaitForSeconds(1f);
            timer--;
            orderDetails.UpdateTime(timer);
        }

        LeaveRestaurant(false);
    }

    public void FulfillOrder(int number)
    {
        if (isOrderFulfilled) return;
        if (number != orderValue) return;

        isOrderFulfilled = true;
        LeaveRestaurant(true);
    }

    public void LeaveRestaurant(bool happy)
    {
        leaving = true;

        if (orderDetails != null && orderDetails.orderContainer != null)
        {
            orderListContainer.Remove(orderDetails.orderContainer);
            orderDetails = null;
        }

        animator.SetBool("Idle", false);
        animator.SetBool("Jump", true);
        Leave();
    }

    public void Leave()
    {
        if (isSeated)
        {
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
            if (Vector3.Distance(transform.position, agent.destination) < 3)
            {
                animator.SetBool("Jump", false);
                animator.SetBool("Idle", true);
                yield return new WaitForSeconds(0.5f);
                spawner.CustomerLeft(seatId);
                Destroy(gameObject);
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsInteractable = true;
            Debug.Log($"[CustomerAI] Player entered interaction range for customer at seat {seatId}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsInteractable = false;
            Debug.Log($"[CustomerAI] Player exited interaction range for customer at seat {seatId}");
        }
    }

}
