using UnityEngine;
using System.Collections;
public class Interactable : MonoBehaviour
{
    public enum InteractionType
    {
        PickUp, // For number items
        Discard, // For trash items
        Operator
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
    public void Interact(PlayerController player)
    {
        if (interactionType == InteractionType.Operator)
        {
            HandleOperatorInteraction(player);
        }
        else if (interactionType == InteractionType.PickUp)
        {
            if (player.HeldNumber != null)
            {
                return;
            }
            gameObject.SetActive(false); // Hide the object
            GameInteractableManager.Instance.DisableTemporarily(gameObject, 3.0f); // Call GameManager to handle re-enabling
        }
        else if (interactionType == InteractionType.Discard)
        {
            Debug.Log("Discarding held number");
            player.DiscardHeldNumber();
        }
    }

    private void HandleOperatorInteraction(PlayerController player)
    {
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

            player.DiscardHeldNumber();
        }
        else if (secondNumber == null)
        {
            secondNumber = player.HeldNumber;
            player.DiscardHeldNumber();

            StartCoroutine(CalculateResult(player));
        }
        // if (player.HeldNumber != null)
        // {
        //     if (!isHoldingNumber)
        //     {
        //         firstNumber = player.HeldNumber;
        //         isHoldingNumber = true;
        //         renderer.material = FilledMaterial;

        //         player.DiscardHeldNumber();

        //     } 
        //     else if (firstNumber != null && secondNumber == null)
        //     {
        //         Debug.Log("Second number stored");
        //         secondNumber = player.HeldNumber;
        //         player.DiscardHeldNumber();

        //         StartCoroutine(CalculateResult(player));
        //     }
        // } 
    }

    private IEnumerator CalculateResult(PlayerController player)
    {
        isCalculating = true;

        yield return new WaitForSeconds(1.0f);

        int result = 0;
        Debug.Log("Calculating result");
        if (gameObject.name.Contains("Addition")) // Example: Cube is for addition
        {
            Debug.Log("Calculating addition" + firstNumber + " + " + secondNumber);
            result = (firstNumber ?? 0) + (secondNumber ?? 0);
        }
        else if (gameObject.name.Contains("Subtraction")) // Example: Sphere is for subtraction
        {
            result = (firstNumber ?? 0) - (secondNumber ?? 0);
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
}