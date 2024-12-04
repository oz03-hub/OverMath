using UnityEngine;
using System.Collections;
public class Interactable : MonoBehaviour
{
    public enum InteractionType
    {
        PickUp, // For number items
        Discard // For trash items
    }
    public InteractionType interactionType;
    public Material highlightMaterial;
    public Material originalMaterial;
    private Renderer renderer; // This is the class-level renderer reference
    private bool isBlinking = false;
    private Coroutine blinkCoroutine;
	public float blinkDuration = 1f;
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
            // Set the initial material to the original material
            renderer.material = originalMaterial;
        }
    }
    public void Highlight(bool highlight)
    {
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
        if (interactionType == InteractionType.PickUp)
        {
            gameObject.SetActive(false); // Hide the object
            GameInteractableManager.Instance.DisableTemporarily(gameObject, 3.0f); // Call GameManager to handle re-enabling
        }
        else if (interactionType == InteractionType.Discard)
        {
            Debug.Log("Discarding held number");
            player.DiscardHeldNumber();
        }
    }
}