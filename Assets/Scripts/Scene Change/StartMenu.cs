using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartMenu : MonoBehaviour
{
  private Button playButton;

  void OnEnable()
  {
    var root = GetComponent<UIDocument>().rootVisualElement;

    playButton = root.Q<Button>("PlayButton");

    playButton.RegisterCallback<ClickEvent>(GoToLevel1);
  
  }

  void OnDisable()
  {
    playButton.UnregisterCallback<ClickEvent>(GoToLevel1);
  }

  private void GoToLevel1(ClickEvent e)
  {
    Debug.Log("Loading Level1");
    SceneManager.LoadScene("Level1");
  }
}