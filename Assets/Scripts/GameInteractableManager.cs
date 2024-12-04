using System.Collections;
using UnityEngine;
public class GameInteractableManager : MonoBehaviour
{
    public static GameInteractableManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    public void DisableTemporarily(GameObject obj, float delay)
    {
        StartCoroutine(ReenableAfterDelay(obj, delay));
    }
    private IEnumerator ReenableAfterDelay(GameObject obj, float delay)
    {
        obj.SetActive(false); // Disable the object
        yield return new WaitForSeconds(delay);
        obj.SetActive(true); // Re-enable the object
    }
}