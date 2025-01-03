using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexStack : MonoBehaviour
{
    public List<Hexagon> hexagons { get; private set; }
    private rocketPowerUp rocketPowerUpScript;
    public int Count => hexagons.Count;
    void Start()
    {
        rocketPowerUpScript = FindObjectOfType<rocketPowerUp>();
    }
    private void Awake()
    {
        hexagons = new List<Hexagon>();
    }

    private void OnMouseDown()
    {
        Debug.Log($"{name} was clicked.");
        // Handle destruction in rocketPowerUp, but no dependency here
    }

    public void DestroyStack()
    {
        Debug.Log($"Destroying HexStack {name}");
        Destroy(gameObject); // Destroys the stack object
    }

    public void Initialize()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Add(transform.GetChild(i).GetComponent<Hexagon>());
        }
        Place();
    }

    public void Add(Hexagon hex)
    {
        if (hex == null)
        {
            Debug.LogError("Attempting to add a null Hexagon to HexStack.");
            return;
        }

        if (hexagons == null)
        {
            hexagons = new List<Hexagon>();
        }

        hexagons.Add(hex);
        hex.SetParent(transform);
        Debug.Log($"Added hexagon {hex.name} to HexStack {name}. Total count: {hexagons.Count}");
    }

    

    public Color GetTopHexColor()
    {
        if (hexagons == null || hexagons.Count == 0)
        {
            Debug.LogError("HexStack is empty or not initialized. Returning default color.");
            return Color.clear;
        }
        return hexagons[^1].color;
    }

    public void Place()
    {
        
        foreach (Hexagon hexagon in hexagons)
        {
            hexagon.DisableCollider();
        }
        if (rocketPowerUpScript == null)
        {
            Debug.LogWarning("rocketPowerUpScript is null. Skipping collider enablement.");
            return;
        }
        if (rocketPowerUpScript.isPowerUpActive == true)
        {
            foreach (Hexagon hex in hexagons)
            {
                hex.EnableCollider();
            }
        }
    }

    public bool Contains(Hexagon hex)
    {
        return hexagons != null && hexagons.Contains(hex);
    }

    public void Remove(Hexagon hex)
    {
        if (hex == null)
        {
            Debug.LogError("Attempting to remove a null Hexagon from HexStack.");
            return;
        }

        if (hexagons == null || !hexagons.Contains(hex))
        {
            Debug.LogWarning("Hexagon not found in HexStack.");
            return;
        }

        hexagons.Remove(hex);
        Debug.Log($"Removed hexagon {hex.name} from HexStack {name}. Remaining count: {hexagons.Count}");

        if (hexagons.Count == 0)
        {
            Debug.Log($"HexStack {name} is empty. Destroying GameObject.");
            Destroy(gameObject);
        }
    }
}
