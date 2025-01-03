using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText; // Assign this in the Inspector
    public int score; // Initial score
    private MergeManager mergeManager; // Reference to the MergeManager script
    private int previousDestroyedHexCount = 0; // Tracks already processed hexagons

    void Start()
    {
        // Find the MergeManager in the scene
        mergeManager = FindObjectOfType<MergeManager>();

        if (mergeManager == null)
        {
            Debug.LogError("MergeManager not found in the scene!");
        }

        // Initialize the score display
        UpdateScoreText();
    }

    void Update()
    {
        // Update the score dynamically based on destroyed hexagons
        if (mergeManager != null)
        {
            int currentDestroyedHexCount = mergeManager.GetDestroyedHexagonCount();
            int newlyDestroyedHexCount = currentDestroyedHexCount - previousDestroyedHexCount;

            if (newlyDestroyedHexCount > 0)
            {
                score = Mathf.Max(score - newlyDestroyedHexCount, 0); // Reduce the score
                previousDestroyedHexCount = currentDestroyedHexCount; // Update the tracked count
                UpdateScoreText(); // Refresh the UI
            }
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = $"{score}";
    }
}
