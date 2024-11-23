using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartMenu : MonoBehaviour
{
  private Button playButton;
  private TextField nameInput;

  void OnEnable()
  {
    var root = GetComponent<UIDocument>().rootVisualElement;

    nameInput = root.Q<TextField>("NameInput");
    playButton = root.Q<Button>("PlayButton");

    playButton.RegisterCallback<ClickEvent>(GoToLevel1);
  
  }

  void OnDisable()
  {
    playButton.UnregisterCallback<ClickEvent>(GoToLevel1);
  }

  private void GoToLevel1(ClickEvent e)
  {
    Debug.Log("Player Name: " + nameInput.value);
    SceneManager.LoadScene("Level1");
  }
}