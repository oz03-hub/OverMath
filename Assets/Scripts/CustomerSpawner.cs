using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject[] customerPrefabs;  // Array of different customer prefabs
    public Transform[] seatPositions;
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

        // Check if any prefabs are null
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

        // Randomly select a customer prefab
        int randomPrefabIndex = Random.Range(0, customerPrefabs.Length);
        GameObject selectedPrefab = customerPrefabs[randomPrefabIndex];
        
        GameObject customer = Instantiate(selectedPrefab, exitDoor.position, Quaternion.identity);
        CustomerAI customerAI = customer.GetComponent<CustomerAI>();
        
        if (customerAI != null)
        {
            customerAI.seatId = seatId;
            customerAI.targetSeat = seatPositions[seatId];
            customerAI.exitDoor = exitDoor;
            customerAI.GameGUI = gameGUI;
            customerAI.quizGeneratorObject = quizGeneratorObject;
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