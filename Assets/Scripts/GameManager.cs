using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public ScoreManager scoreManager; // Reference to the ScoreManager
    public GameObject levelCompleteCanvas; // Assign this in the Inspector

    void Awake()
    {
        // Ensure the level complete canvas is inactive at the start
        if (levelCompleteCanvas != null)
        {
            levelCompleteCanvas.SetActive(false);
            foreach(Transform child in levelCompleteCanvas.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("Level Complete Canvas is not assigned!");
        }
    }

    void Update()
    {
        // Check the score in every frame
        if (scoreManager != null && scoreManager.score <= 0)
        {
            ActivateLevelCompleteCanvas();
        }
    }

    private void ActivateLevelCompleteCanvas()
    {
        if (levelCompleteCanvas != null && !levelCompleteCanvas.activeSelf)
        {
            levelCompleteCanvas.SetActive(true);
            foreach (Transform child in levelCompleteCanvas.transform)
            {
                child.gameObject.SetActive(true);
            }
            Debug.Log("Level Complete!");
        }
    }

    public void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if the next scene index is valid
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels to load!");
        }
    }
}
