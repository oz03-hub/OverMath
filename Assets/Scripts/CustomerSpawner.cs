using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject[] customerPrefabs;
    public Transform[] seatPositions;
    public float[] waitTimeRange = new float[] { 30f, 60f };
    public Transform exitDoor;
    public float spawnInterval = 10f;
    public int maxCustomers = 8;
    public UIDocument gameGUI;
    public GameObject quizGeneratorObject;
    
    private float nextSpawnTime;
    public List<int> availableSeats;
    private int customersActive = 0;

    void Start()
    {
        Debug.Log("[CustomerSpawner] Starting...");
        
        if (customerPrefabs == null || customerPrefabs.Length == 0)
        {
            Debug.LogError("[CustomerSpawner] No customer prefabs assigned!");
            enabled = false;
            return;
        }

        for (int i = 0; i < customerPrefabs.Length; i++)
        {
            if (customerPrefabs[i] == null)
            {
                Debug.LogError($"[CustomerSpawner] Customer prefab at index {i} is null!");
                enabled = false;
                return;
            }
        }

        if (seatPositions == null || seatPositions.Length == 0)
        {
            Debug.LogError("[CustomerSpawner] No seat positions assigned!");
            enabled = false;
            return;
        }

        if (exitDoor == null)
        {
            Debug.LogError("[CustomerSpawner] Exit door not assigned!");
            enabled = false;
            return;
        }

        if (gameGUI == null)
        {
            Debug.LogError("[CustomerSpawner] GameGUI not assigned!");
            enabled = false;
            return;
        }

        if (quizGeneratorObject == null)
        {
            Debug.LogError("[CustomerSpawner] QuizGenerator not assigned!");
            enabled = false;
            return;
        }

        nextSpawnTime = Time.time + Random.Range(2f, spawnInterval);
        InitializeAvailableSeats();
        Debug.Log("[CustomerSpawner] Initialized with " + seatPositions.Length + " seats and " + customerPrefabs.Length + " customer types.");
    }

    void Update()
    {
        if (availableSeats == null || availableSeats.Count == 0) 
        {
            return;
        }
        
        if (Time.time >= nextSpawnTime && customersActive < maxCustomers && availableSeats.Count > 0)
        {
            Debug.Log("[CustomerSpawner] Spawning new customer. Active: " + customersActive);
            SpawnCustomer();
            nextSpawnTime = Time.time + Random.Range(spawnInterval * 0.5f, spawnInterval * 1.5f);
        }
    }

    private void InitializeAvailableSeats()
    {
        availableSeats = new List<int>();
        for (int i = 0; i < seatPositions.Length; i++)
        {
            availableSeats.Add(i);
        }
    }

    private void SpawnCustomer()
    {
        if (availableSeats.Count == 0) return;

        int randomSeatIndex = Random.Range(0, availableSeats.Count);
        int seatId = availableSeats[randomSeatIndex];
        availableSeats.Remove(seatId);

        int randomPrefabIndex = Random.Range(0, customerPrefabs.Length);
        GameObject selectedPrefab = customerPrefabs[randomPrefabIndex];

        GameObject customerObject = Instantiate(selectedPrefab, exitDoor.position, Quaternion.identity);
        

        customerObject.layer = LayerMask.NameToLayer("Interactable");

        Collider collider = customerObject.GetComponent<Collider>();
        if (collider == null)
        {
            SphereCollider trigger = customerObject.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = 1.5f;
        }

        Renderer renderer = customerObject.GetComponent<Renderer>();
        if (renderer == null)
        {
            MeshRenderer meshRenderer = customerObject.AddComponent<MeshRenderer>();
        }

        Interactable interactable = customerObject.AddComponent<Interactable>();
        if (interactable == null)
        {
            interactable = customerObject.AddComponent<Interactable>();
        }
        interactable.interactionType = Interactable.InteractionType.Customer;

        Material highlightMaterial = Resources.Load<Material>("Materials/HighlightCustomer");
        Material filledMaterial = Resources.Load<Material>("Materials/FilledCustomer");
        Material originalMaterial = Resources.Load<Material>("Materials/OriginalCustomer");

        if (highlightMaterial != null)
        {
            interactable.highlightMaterial = highlightMaterial;
        }

        if (filledMaterial != null)
        {
            interactable.FilledMaterial = filledMaterial;
        }

        if (originalMaterial != null)
        {
            interactable.originalMaterial = originalMaterial;

            if (renderer != null)
            {
                renderer.material = originalMaterial;
            }
        }

        CustomerAI customerAI = customerObject.GetComponent<CustomerAI>();
        customerAI.waitTime = Random.Range(waitTimeRange[0], waitTimeRange[1]);
        if (customerAI != null)
        {
            customerAI.seatId = seatId;
            customerAI.targetSeat = seatPositions[seatId];
            customerAI.exitDoor = exitDoor;
            customerAI.GameGUI = gameGUI;
            customerAI.quizGeneratorObject = quizGeneratorObject;

            interactable.AssignCustomer(customerAI);

            customersActive++;
            Debug.Log($"[CustomerSpawner] Spawned customer type {randomPrefabIndex} at seat {seatId}");
        }
    }


    public void CustomerLeft(int seatId)
    {
        if (!availableSeats.Contains(seatId))
        {
            availableSeats.Add(seatId);
        }
        customersActive--;
    }
} 