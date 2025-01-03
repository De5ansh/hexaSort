using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class rocketPowerUp : MonoBehaviour
{
    [Header("Power-up Settings")]
    [SerializeField] private Vector3 scaledSize = new Vector3(1.5f, 1.5f, 1.5f);
    [SerializeField] private float raiseHeight = 0.5f;
    [SerializeField] private float destroyDelay = 0.03f;

    [Header("State Management")]
    public bool isPowerUpActive = false;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Camera mainCamera;

    private void Start()
    {
        InitializePowerUp();
    }

    private void InitializePowerUp()
    {
        mainCamera = Camera.main;
        originalScale = transform.localScale;
        originalPosition = transform.position;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Hexagon hex = hit.collider.GetComponent<Hexagon>();
            if (hex != null && isPowerUpActive)
            {
                Debug.Log("Hit hex, destroying stack...");
                StartCoroutine(DestroyStackSequence(hex));  // Destroy stack only if power-up is active
            }
            else if (hit.collider.CompareTag("rocket"))
            {
                TogglePowerUp();  // Toggle power-up state on rocket collider click
            }
        }
    }

    private void TogglePowerUp()
    {
        if (isPowerUpActive)
        {
            // If the power-up is active, deactivate it
            DeactivatePowerUp();
        }
        else
        {
            // If the power-up is inactive, activate it
            ActivatePowerUp();
        }
    }

    private void ActivatePowerUp()
    {
        if (!isPowerUpActive)  // Check to ensure it isn't already activated
        {
            isPowerUpActive = true;
            ApplyPowerUpEffects();
            Debug.Log("Power-up activated.");
        }
    }

    private void DeactivatePowerUp()
    {
        if (isPowerUpActive)  // Check to ensure it isn't already deactivated
        {
            isPowerUpActive = false;
            ResetPowerUpEffects();
            Debug.Log("Power-up deactivated.");
        }
    }


    private void ApplyPowerUpEffects()
    {
        transform.localScale = scaledSize;
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y + raiseHeight,
            transform.position.z
        );
    }

    private void ResetPowerUpEffects()
    {
        transform.localScale = originalScale;
        transform.position = originalPosition;
    }

    private IEnumerator DestroyStackSequence(Hexagon hex)
    {
        if (hex == null) yield break;

        // Access the HexStack from the clicked Hexagon
        HexStack hexStack = hex.hex;
        if (hexStack == null || hexStack.Count == 0) yield break;

        // Check if the hexagon is in an occupied GridCell
        GridCell gridCell = hex.transform.GetComponentInParent<GridCell>();
        if (gridCell == null || !gridCell.isOccupied)
        {
            Debug.Log("No stack in this grid cell. Power-up will not work.");
            yield break;  // Exit the sequence if the grid cell is not occupied
        }

        // Proceed with destroying the hex stack
        while (hexStack.Count > 0)
        {
            Hexagon topHex = hexStack.hexagons[hexStack.hexagons.Count - 1]; // Get the last hexagon in the list
            hexStack.Remove(topHex); // Remove the hexagon from the stack list

            // Vanish the hexagon before destroying
            topHex.Vanish(0.1f);  // Apply vanish effect before destroying

            // Wait before destroying the next hexagon
            yield return new WaitForSeconds(destroyDelay);
        }

        // Wait for any animations to finish and then deactivate power-up
        yield return new WaitForSeconds(0.2f);
        DeactivatePowerUp();
    }


    // Wait for any animations to finish and then deactivate power-up
    

}

