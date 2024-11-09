using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level2Button : MonoBehaviour
{
    public GameLevelManager gameLevelManager;
    public Button level2Button;

    void Start()
    {
        level2Button.gameObject.SetActive(false);
        level2Button.onClick.AddListener(LoadNextLevel);
    }

    void Update()
    {
        if (gameLevelManager.hasWon && !level2Button.gameObject.activeSelf)
        {
            level2Button.gameObject.SetActive(true);
        }
    }

    void LoadNextLevel()
    {
        SceneManager.LoadScene("Level2");
    }
}
