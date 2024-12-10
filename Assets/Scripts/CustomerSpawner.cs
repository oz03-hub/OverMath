using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;
    public Transform[] seatPositions;  // Assign seat transforms in inspector
    public Transform exitDoor;      // Assign door transform in inspector
    public float spawnInterval = 20f;   // Time between spawns
    public int maxCustomers = 8;        // Maximum concurrent customers
    public UIDocument gameGUI;  // Reference to the shared GameGUI
    public GameObject quizGeneratorObject; // Reference to QuizGenerator
    
    private float nextSpawnTime;
    private List<int> availableSeats;
    private int customersActive = 0;

    void Start()
    {
        if (customerPrefab == null)
        {
            Debug.LogError("[CustomerSpawner] Customer prefab not assigned!");
            return;
        }

        nextSpawnTime = Time.time + Random.Range(2f, spawnInterval);
        InitializeAvailableSeats();
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime && customersActive < maxCustomers && availableSeats.Count > 0)
        {
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
        availableSeats.RemoveAt(randomSeatIndex);

        GameObject customer = Instantiate(customerPrefab, exitDoor.position, Quaternion.identity);
        CustomerAI customerAI = customer.GetComponent<CustomerAI>();
        
        if (customerAI != null)
        {
            customerAI.seatId = seatId;
            customerAI.targetSeat = seatPositions[seatId];
            customerAI.exitDoor = exitDoor;
            customerAI.GameGUI = gameGUI;
            customerAI.quizGeneratorObject = quizGeneratorObject;
            customersActive++;
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