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

	private Vector3[] spawnPositions = new Vector3[] { new Vector3(-14.77f, 2.5f, -9.7f), 
													  new Vector3(-12.5f, 2.5f, -9.7f), 
													  new Vector3(-10.19f, 2.5f, -9.7f), 
													  new Vector3(-4.1f, 2.8f, -18.7f),
													  new Vector3(-2.5f, 2.8f, -18.7f),
													  new Vector3(-0.9f, 2.8f, -18.7f),
													  new Vector3(0.4f, 2.8f, -18.7f),
													  new Vector3(1.29f, 2.8f, -18.7f),
													  new Vector3(2.1f, 2.8f, -18.7f),
													  };
	public QuizGenerator quizGenerator;
	private Interactable interactable;
	private int[] allowedNumbers;
	public Vector3 spawnPositionAdditive = new Vector3(11.89f, 2.5f, -9.7f);
	public Vector3 spawnPositionSubtractive = new Vector3(14.27f, 2.5f, -9.7f);

	public Vector3 numberScale = new Vector3(70f, 70f, 70f);
	public Vector3 numberRotation = new Vector3(0f, 0f, -90f);

	public Vector3 operationScale = new Vector3(0.7f, 0.7f, 0.7f);

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
		GameObject additive = GameObject.CreatePrimitive(PrimitiveType.Cube);
		GameObject subtractive = GameObject.CreatePrimitive(PrimitiveType.Sphere);

		additive.name = "Addition";
		subtractive.name = "Subtraction";

		additive.transform.position = spawnPositionAdditive;
		subtractive.transform.position = spawnPositionSubtractive;

		additive.transform.localScale = operationScale;
		subtractive.transform.localScale = operationScale;

		int interactableLayer = LayerMask.NameToLayer("Interactable");
		additive.layer = interactableLayer;
		subtractive.layer = interactableLayer;

		additive.AddComponent<BoxCollider>();
		subtractive.AddComponent<BoxCollider>();

		Interactable additiveInteractable = additive.AddComponent<Interactable>();
		additiveInteractable.interactionType = Interactable.InteractionType.Operator;

		Interactable subtractiveInteractable = subtractive.AddComponent<Interactable>();
		subtractiveInteractable.interactionType = Interactable.InteractionType.Operator;

		Material highlightMaterial = Resources.Load<Material>("Materials/HighlightMaterial");
		Material filledMaterial = Resources.Load<Material>("Materials/FilledMaterial");

		if (highlightMaterial != null)
		{
			additiveInteractable.highlightMaterial = highlightMaterial;
			subtractiveInteractable.highlightMaterial = highlightMaterial;
		}

		if (filledMaterial != null)
		{
			additiveInteractable.FilledMaterial = filledMaterial;
			subtractiveInteractable.FilledMaterial = filledMaterial;
		}

		additiveInteractable.originalMaterial = Resources.Load<Material>("Materials/MockMaterial");
		subtractiveInteractable.originalMaterial = Resources.Load<Material>("Materials/MockMaterial");

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