using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GridGenerator : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject hexagon;

    [Header("Settings")]
    [OnValueChanged("GenerateGrid")]
    [SerializeField] private int gridSize;

    private void GenerateGrid()
    {
        if (grid == null || hexagon == null)
        {
            Debug.LogError("Grid or Hexagon is not assigned in the Inspector!");
            return;
        }

        transform.ClearChildren();

        List<GridCell> gridCells = new List<GridCell>();

        for (int i = -gridSize; i <= gridSize; i++)
        {
            for (int j = -gridSize; j <= gridSize; j++)
            {
                Vector3 spawnPos = grid.CellToWorld(new Vector3Int(i, j, 0));
                if (spawnPos.magnitude > grid.CellToWorld(new Vector3Int(1, 0, 0)).magnitude * gridSize)
                {
                    continue;
                }

                GameObject cellObj = Instantiate(hexagon, spawnPos, Quaternion.identity, transform);
                GridCell gridCell = cellObj.GetComponent<GridCell>();
                gridCells.Add(gridCell);
            }
        }

        
        FindObjectOfType<MergeManager>()?.RegisterInitialStacks(gridCells.ToArray());

        
        transform.rotation = Quaternion.Euler(0, 0, 90);
    }

}

public static class TransformExtension
{
    public static void ClearChildren(this Transform transform)
    {
        while (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            child.SetParent(null);
            Object.DestroyImmediate(child.gameObject); 
        }
    }
}
