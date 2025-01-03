using UnityEngine;
using UnityEngine.SceneManagement;
public class PowerUpsManager : MonoBehaviour
{
    [System.Serializable]
    public class PowerUp
    {
        public string name; // Name of the power-up
        public GameObject questionMesh; // Placeholder object (e.g., question mark)
        public GameObject powerUpObject; // Actual power-up object (e.g., rocket)
        public int unlockLevel; // Level at which this power-up is unlocked
    }

    public PowerUp[] powerUps; // List of all power-ups
    private int currentLevel; // Track the current level

    void Start()
    {
        currentLevel = GetCurrentLevel();
        UpdatePowerUps();
    }

    private int GetCurrentLevel()
    {
        // Use Unity's SceneManager to determine the current level
        return SceneManager.GetActiveScene().buildIndex + 1; // Assuming level index starts from 0
    }

    private void UpdatePowerUps()
    {
        foreach (PowerUp powerUp in powerUps)
        {
            if (currentLevel >= powerUp.unlockLevel)
            {
                UnlockPowerUp(powerUp);
            }
        }
    }

    private void UnlockPowerUp(PowerUp powerUp)
    {
        if (powerUp.questionMesh != null)
        {
            powerUp.questionMesh.SetActive(false); // Hide the placeholder mesh
        }

        if (powerUp.powerUpObject != null)
        {
            powerUp.powerUpObject.SetActive(true); // Activate the power-up object
            Debug.Log($"Unlocked power-up: {powerUp.name}");
        }
    }
}
