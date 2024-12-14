using UnityEngine;

public class OperationSpawner : MonoBehaviour
{
	private string[] prefabNames = new string[] { "Number0", 
												 "Number1", 
												 "Number2", 
												 "Number3", 
												 "Number4", 
												 "Number5", 
												 "Number6", 
												 "Number7", 
												 "Number8", 
												 "Number9" };
	private string[] prefabNamesOperator = new string[] { "PlusPan", 
														  "MinusCuttingBoard" 
														};
	private Vector3[] spawnPositions = new Vector3[] { new Vector3(-14.77f, 2.5f, -9.7f), 
													  new Vector3(-12.5f, 2.5f, -9.7f), 
													  new Vector3(-10.19f, 2.5f, -9.7f), 
													  new Vector3(-4.1f, 2.8f, -18.7f),
													  new Vector3(-2.29f, 2.8f, -18.7f),
													  new Vector3(-0.9f, 2.8f, -18.7f),
													  new Vector3(0.4f, 2.8f, -18.7f),
													  new Vector3(1.29f, 2.8f, -18.7f),
													  new Vector3(2.1f, 2.8f, -18.7f),
													  };
	private Vector3[] operatorSpawnPosition = new Vector3[] {new Vector3(11.89f, 2.5f, -9.7f),
															 new Vector3(14.27f, 2.466f, -9.7f),
															 new Vector3(16.53f, 2.46f, -9.7f)
															};
	private Vector3[] operatorSpawnRotation = new Vector3[] {new Vector3(-90f, 0f, 0f),
															 new Vector3(0f, 0f, -90f)
															};
	public QuizGenerator quizGenerator;
	private Interactable interactable;
	private int[] allowedNumbers;
	// public Vector3 spawnPositionAdditive = new Vector3(11.89f, 2.5f, -9.7f);
	// public Vector3 spawnPositionSubtractive = new Vector3(14.27f, 2.466f, -9.7f);

	public Vector3 numberScale = new Vector3(70f, 70f, 70f);
	public Vector3 numberRotation = new Vector3(0f, 0f, -90f);

	// public Interactable interactable;

	void Start()
	{
		GetAllowedNumbers();
		SpawnOperation();
		SpawnNumbers();
	}

	/**
	* Get allowed numbers from QuizGenerator
	* @param: none
	* @return: void
	*/
	void GetAllowedNumbers()
	{
		if (quizGenerator != null)
		{
			Debug.Log("QuizGenerator found!");
			allowedNumbers = quizGenerator.allowedNumbers;
		}
		else
		{
			Debug.LogError("QuizGenerator not found!");
		}
	}

	/**
	 * Spawns operation at the specified positions
	 * @param: none
	 * @return: void
	 */
	void SpawnOperation()
	{
		void ApplyOperatorPrefabProperties(string prefabName, int i)
		{
			GameObject operationPrefab = Resources.Load<GameObject>("Operators/" + prefabName);

			GameObject operation = Instantiate(operationPrefab, operatorSpawnPosition[i], Quaternion.identity);

			operation.name = prefabName;

			operation.transform.Rotate(operatorSpawnRotation[i]);

			operation.transform.localScale = new Vector3(120f, 120f, 120f);

			int interactableLayer = LayerMask.NameToLayer("Interactable");
			operation.layer = interactableLayer;

			operation.AddComponent<BoxCollider>();

			Interactable operationInteractable = operation.AddComponent<Interactable>();
			operationInteractable.interactionType = Interactable.InteractionType.Operator;

			Material highlightMaterial = Resources.Load<Material>("Materials/HighlightMaterial");
			Material filledMaterial = Resources.Load<Material>("Materials/FilledMaterial");

			if (highlightMaterial != null)
			{
				operationInteractable.highlightMaterial = highlightMaterial;
			}

			if (filledMaterial != null)
			{
				operationInteractable.FilledMaterial = filledMaterial;
			}

			operationInteractable.originalMaterial = Resources.Load<Material>("Materials/" + prefabName);
		}

		for (int i = 0; i < prefabNamesOperator.Length; i++)
		{
			ApplyOperatorPrefabProperties(prefabNamesOperator[i], i);
		}

		Debug.Log("Operation spawned!");
	}

	/**
	 * Spawns numbers at the specified positions
	 * @param: none
	 * @return: void
	 */
	void SpawnNumbers()
	{
		/**
		 * Helper function to apply properties to number prefab
		 * [CMT: I decided to write inside here to separate the logic of main functions and helper functions]
		 */
		void ApplyNumberPrefabProperties(GameObject numberPrefab, Vector3 spawnPositionNumber, int i)
		{
			GameObject number = Instantiate(numberPrefab, spawnPositionNumber, Quaternion.identity);
			number.transform.localScale = numberScale;
			number.transform.Rotate(numberRotation);

			// Assign the INteractable Layer
			int interactableLayer = LayerMask.NameToLayer("Interactable");
			number.layer = interactableLayer;
			number.AddComponent<BoxCollider>();

			// Add the interactable script and configure it
			Interactable interactable = number.AddComponent<Interactable>();
			interactable.originalMaterial = Resources.Load<Material>("Materials/" + prefabNames[i]);

			// Add and aconfigure the NumberComponent script
			NumberComponent numberComponent = number.AddComponent<NumberComponent>();
			numberComponent.numberValue = allowedNumbers[i];

			Material highlightMaterial = Resources.Load<Material>("Materials/HighlightMaterial");
			if (highlightMaterial != null)
			{
				Debug.Log("Highlight material found!");
				interactable.highlightMaterial = highlightMaterial;
			}
			else
			{
				Debug.LogError("Highlight material not found!");
			}
			interactable.blinkDuration = 1.0f;

		}
		// End of helper function

		// Starts of spawning numbers
		for (int i = 0; i < allowedNumbers.Length; i++)
		{
			GameObject numberPrefab = Resources.Load<GameObject>("Numbers/" + prefabNames[allowedNumbers[i]]); // Import dynamically
			ApplyNumberPrefabProperties(numberPrefab, spawnPositions[i], i);
		}
	}
}