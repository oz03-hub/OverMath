using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class RestartGame : MonoBehaviour
{
    private Button exitButton;

    void OnEnable()
    {
        var document = GetComponent<UIDocument>();
        if (document == null)
        {
            Debug.LogError("UIDocument component not found on this GameObject.");
            return;
        }

        var root = document.rootVisualElement;
        exitButton = root.Q<Button>("ExitButton");

        if (exitButton != null)
        {
            exitButton.RegisterCallback<ClickEvent>(GoToInstructionScene);
        }
        else
        {
            Debug.LogError("ExitButton not found in the UI.");
        }
    }

    void OnDisable()
    {
        if (exitButton != null)
        {
            exitButton.UnregisterCallback<ClickEvent>(GoToInstructionScene);
        }
        else
        {
            Debug.LogError("ExitButton is null, cannot unregister callback.");
        }
    }

    private void GoToInstructionScene(ClickEvent e)
    {
        Debug.Log("Loading Scene 0");
        SceneManager.LoadScene(0);
    }
}