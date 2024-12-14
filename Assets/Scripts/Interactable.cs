using UnityEngine;
using System.Collections;
public class Interactable : MonoBehaviour
{
    public enum InteractionType
    {
        PickUp, // For number items
        Discard, // For trash items
        Operator, // For operator items
        Customer // For customer interaction 
    }
    public InteractionType interactionType;
    public Material highlightMaterial;
    public Material originalMaterial;
    public Material FilledMaterial;
	public float blinkDuration = 1f;
    private Renderer renderer; // This is the class-level renderer reference
    private bool isBlinking = false;
    private Coroutine blinkCoroutine;
    private bool isHoldingNumber = false;
    private CustomerAI associateCustomer;

    private int? firstNumber = null;
    private int? secondNumber = null;
    private bool isCalculating = false;

    // public GameObject numberPrefab;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        
        if (renderer != null)
        {
            // Set the current material as the original material if it's not assigned in the Inspector
            if (originalMaterial == null)
            {
                originalMaterial = renderer.material;
            }
            // Set the initial material to the original materialW
            renderer.material = originalMaterial;
        }
    }
    public void Highlight(bool highlight)
    {
        // Prevent blinking if the object is holding a number
        if (isCalculating) return;
        if (isHoldingNumber)
        {
            StopBlinking();
            renderer.material = FilledMaterial;
            return;
        }

        if (highlight && !isBlinking) 
        {
            isBlinking = true;
            blinkCoroutine = StartCoroutine(SmoothMaterialBlink());
            // blinkCoroutine = StartCoroutine(BlinkEffect());
        }
        else if (!highlight && isBlinking) 
        {
            isBlinking = false;
            
            if (blinkCoroutine != null) 
            {
                StopCoroutine(blinkCoroutine);
            }
            renderer.material = originalMaterial; // Revert to the original material
        }
    }
	private IEnumerator SmoothMaterialBlink()
	{
		while (isBlinking)
		{
			// Fade to highlight material
			for (float t = 0; t < blinkDuration; t += Time.deltaTime)
			{
				float lerpFactor = t / blinkDuration;
				renderer.material.Lerp(originalMaterial, highlightMaterial, lerpFactor);
				yield return null;
			}
			// Fade back to original material
			for (float t = 0; t < blinkDuration; t += Time.deltaTime)
			{
				float lerpFactor = t / blinkDuration;
				renderer.material.Lerp(highlightMaterial, originalMaterial, lerpFactor);
				yield return null;
			}
		}
	}
    public void Interact(PlayerController player, NumberComponent number)
    {
        if (interactionType == InteractionType.Operator)
        {
            HandleOperatorInteraction(player);
        }
        else if (interactionType == InteractionType.PickUp)
        {
            if (player.HeldNumber != null || number == null)
            {
                Debug.Log($"[Interactable {interactionType}] Not picking up, already holding");
                return;
            }
            player.PickUpNumber(number);

            gameObject.SetActive(false); // Hide the object
            GameInteractableManager.Instance.DisableTemporarily(gameObject, 3.0f); // Call GameManager to handle re-enabling
        }
        else if (interactionType == InteractionType.Discard)
        {
            Debug.Log("[Interactable] Discarding held number");
            player.DiscardHeldNumber();
        } 
        else if (interactionType == InteractionType.Customer)
        {
            CustomerManager customerManager = FindObjectOfType<CustomerManager>();
            if (player.GetHeldNumber() == -1)
            {
                Debug.LogWarning("[Interactable] Cannot serve customer. Player is not holding a number.");
                return;
            }
            if (customerManager == null)
            {
                Debug.LogError("[Interactable] CustomerManager not found in scene!");
                return;
            }

            if (associateCustomer == null)
            {
                Debug.LogError("[Interactable] Customer not found!");
                return;
            }

            customerManager.HandleCustomerInteraction(associateCustomer, player.HeldNumber ?? -1);
            player.DiscardHeldNumber();
        }
    }

    private void HandleOperatorInteraction(PlayerController player)
    {
        Debug.Log("Ditme t deptraivainoi");
        if (isCalculating)
        {
            return;
        }

        if (player.HeldNumber == null)
        {
            return;
        }

        if (firstNumber == null)
        {
            firstNumber = player.HeldNumber;
            isHoldingNumber = true;
            renderer.material = FilledMaterial;
            Debug.Log($"[Interactable] Storing {firstNumber} as first number");

            player.DiscardHeldNumber();

            if (gameObject.name.Contains("PlusPan"))
            {
                player.UpdateIntText($"{firstNumber}+");
            }
            else if (gameObject.name.Contains("MinusCuttingBoard"))
            {
                player.UpdateIntText($"{firstNumber}-");
            }
            else if (gameObject.name.Contains("MultiplyMicrowave"))
            {
                player.UpdateIntText($"{firstNumber}*");
            }
        }
        else if (secondNumber == null)
        {
            secondNumber = player.HeldNumber;
            player.DiscardHeldNumber();
            Debug.Log($"[Interactable] Storing {secondNumber} as second number");

            StartCoroutine(CalculateResult(player));
        }
    }

    private IEnumerator CalculateResult(PlayerController player)
    {
        isCalculating = true;

        yield return new WaitForSeconds(.2f);

        int result = 0;
        Debug.Log($"[Interactable] Calculating result");
        if (gameObject.name.Contains("PlusPan")) // Example: Cube is for addition
        {
            Debug.Log($"[Interactable] Calculating addition {firstNumber} + {secondNumber}");
            result = (firstNumber ?? 0) + (secondNumber ?? 0);
        }
        else if (gameObject.name.Contains("MinusCuttingBoard")) // Example: Sphere is for subtraction
        {
            Debug.Log($"[Interactable] Calculating substraction {firstNumber} - {secondNumber}");
            result = (firstNumber ?? 0) - (secondNumber ?? 0);
        } else if (gameObject.name.Contains("MultiplyMicrowave"))
        {
            Debug.Log($"[Interactable] Calculating multiplication {firstNumber} * {secondNumber}");
            result = (firstNumber ?? 0) * (secondNumber ?? 0);
        }

        player.SetHeldNumber(result);

        ResetOperator();
    }

    private void ResetOperator()
    {
        firstNumber = null;
        secondNumber = null;
        isHoldingNumber = false;
        isCalculating = false;
        renderer.material = originalMaterial;
    }

    public int? RetrieveStoredNumber()
    {
        return firstNumber;
    }

    public bool IsHoldingNumber()
    {
        return isHoldingNumber;
    }

    private void StopBlinking()
    {
        isBlinking = false;

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        renderer.material = originalMaterial; // Revert to the original material (if not holding a number)
    }

    public void AssignCustomer(CustomerAI customer)
    {
        associateCustomer = customer;
    }
}