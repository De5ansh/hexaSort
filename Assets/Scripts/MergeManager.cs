using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeManager : MonoBehaviour
{
    private int destroyedHexCount = 0;
    private List<GridCell> updatedCells = new List<GridCell>();

    void Awake()
    {
        StackController.onStackPlaced += StackCallBack;
    }

    void OnDestroy()
    {
        StackController.onStackPlaced -= StackCallBack;
    }

    private void StackCallBack(GridCell gridCell)
    {
        StartCoroutine(StackCallBackCoroutine(gridCell));
    }

    IEnumerator StackCallBackCoroutine(GridCell gridCell)
    {
        updatedCells.Add(gridCell);

        while (updatedCells.Count > 0)
        {
            yield return CheckForMerge(updatedCells[0]);
        }
    }

    IEnumerator CheckForMerge(GridCell gridCell)
    {
        updatedCells.Remove(gridCell);
        if (!gridCell.isOccupied)
        {
            yield break;
        }

        LayerMask gridMask = 1 << gridCell.gameObject.layer;
        List<GridCell> gridCellNeighbours = new List<GridCell>();
        Collider[] neighbourGridCellColliders = Physics.OverlapSphere(gridCell.transform.position, 2, gridMask);
        foreach (Collider col in neighbourGridCellColliders)
        {
            GridCell neighbourGridCell = col.GetComponent<GridCell>();
            Debug.Log("Neighbour1");
            if (!neighbourGridCell.isOccupied) continue;
            if (neighbourGridCell == gridCell) continue;
            gridCellNeighbours.Add(neighbourGridCell);
            if (gridCellNeighbours.Count <= 0)
            {
                yield break;
            }
        }

        Color gridCellTopColor = gridCell.Stack.GetTopHexColor();
        Debug.Log($"Checking grid cell {gridCell.name} with top color {gridCellTopColor}");

        List<GridCell> siimilarColorNeighbourCells = new List<GridCell>();
        foreach (GridCell neighbourGridCell in gridCellNeighbours)
        {
            Color topColor = neighbourGridCell.Stack.GetTopHexColor();
            if (gridCellTopColor == topColor)
            {
                siimilarColorNeighbourCells.Add(neighbourGridCell);
            }
        }
        if (siimilarColorNeighbourCells.Count <= 0) { yield break; }
        updatedCells.AddRange(siimilarColorNeighbourCells);

        List<Hexagon> hexagonsToAdd = new List<Hexagon>();
        foreach (GridCell neighbourCell in siimilarColorNeighbourCells)
        {
            HexStack neighbourCellStack = neighbourCell.Stack;
            for (int i = neighbourCellStack.hexagons.Count - 1; i >= 0; i--)
            {
                Hexagon hex = neighbourCellStack.hexagons[i];
                if (hex.color != gridCellTopColor)
                {
                    break;
                }
                hexagonsToAdd.Add(hex);
                hex.SetParent(null);
            }
        }

        foreach (GridCell neighbourCell in siimilarColorNeighbourCells)
        {
            HexStack stack = neighbourCell.Stack;
            foreach (Hexagon hex in hexagonsToAdd)
            {
                if (stack.Contains(hex))
                {
                    stack.Remove(hex);
                }
            }
        }

        float initialY = gridCell.Stack.hexagons.Count * .2f;
        for (int i = 0; i < hexagonsToAdd.Count; i++)
        {
            Hexagon hex = hexagonsToAdd[i];
            float targetY = initialY + i * .2f;
            Vector3 targetLocalPos = Vector3.up * targetY;
            gridCell.Stack.Add(hex);
            hex.MoveLocal(targetLocalPos);
        }
        yield return new WaitForSeconds(0.2f + (hexagonsToAdd.Count + 1) * .01f);

        yield return CheckForCompleteStack(gridCell, gridCellTopColor);
    }

    private IEnumerator CheckForCompleteStack(GridCell gridCell, Color gridCellTopColor)
    {
        if (gridCell.Stack.hexagons.Count < 10)
        {
            yield break;
        }

        List<Hexagon> similarHexagons = GetMatchingTopHexagons(gridCell, gridCellTopColor);

        if (similarHexagons.Count >= 10)
        {
            yield return DestroyMatchingHexagons(gridCell, similarHexagons);
            updatedCells.Add(gridCell);
        }
    }

    private List<Hexagon> GetMatchingTopHexagons(GridCell gridCell, Color targetColor)
    {
        List<Hexagon> similarHexagons = new List<Hexagon>();
        for (int i = gridCell.Stack.hexagons.Count - 1; i >= 0; i--)
        {
            Hexagon hex = gridCell.Stack.hexagons[i];
            if (hex.color != targetColor)
            {
                break;
            }
            similarHexagons.Add(hex);
        }
        return similarHexagons;
    }

    private IEnumerator DestroyMatchingHexagons(GridCell gridCell, List<Hexagon> hexagonsToDestroy)
    {
        float delay = 0;
        int hexagonCount = hexagonsToDestroy.Count;

        while (hexagonsToDestroy.Count > 0)
        {
            Hexagon hexToDestroy = hexagonsToDestroy[0];
            hexToDestroy.SetParent(null);
            hexToDestroy.Vanish(delay);
            delay += 0.03f;
            gridCell.Stack.Remove(hexToDestroy);
            destroyedHexCount++;
            hexagonsToDestroy.RemoveAt(0);
        }

        Debug.Log($"{destroyedHexCount} hexagons");
        yield return new WaitForSeconds(.2f * (hexagonCount + 1) * 0.01f);
    }

    public int GetDestroyedHexagonCount()
    {
        return destroyedHexCount;
    }

    public void RegisterInitialStacks(GridCell[] gridCells)
    {
        foreach (GridCell gridCell in gridCells)
        {
            if (gridCell.isOccupied)
            {
                updatedCells.Add(gridCell);
            }
        }
    }
}