using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackController : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Settings")]
    [SerializeField] private LayerMask hexagonLayerMask;
    [SerializeField] private LayerMask gridHexagonMask;
    [SerializeField] private LayerMask groundMask;
    private rocketPowerUp rocketPowerUpScript;
    private HexStack curHexStack;
    private Vector3 curHexStackInitialPos;
    private GridCell targetCell;

    [Header("Actions")]
    public static Action<GridCell> onStackPlaced;
 
    void Start()
    {
        rocketPowerUpScript = FindObjectOfType<rocketPowerUp>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ManageMouseDown();
        }
        else if(Input.GetMouseButton(0) && curHexStack!=null)
        {
            ManageMouse();
        }
        else if (Input.GetMouseButtonUp(0) && curHexStack!=null)
        {
            ManageMouseUp();
        }
    }

    void ManageMouse()
    {
        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 500, gridHexagonMask);
        if (hit.collider == null)
        {
            
            DraggingAboveGround();
        } else
        {
            
            DraggingAboveGridCell(hit);
        }
    }

    void ManageMouseDown()
    {
        RaycastHit hit;
        // Check if rocket power-up is active or not, and modify the layer mask accordingly
        LayerMask activeLayerMask = rocketPowerUpScript != null && rocketPowerUpScript.isPowerUpActive
            ? (LayerMask.GetMask("Default"))  // Or any other layer you want to interact with
            : (hexagonLayerMask | gridHexagonMask); // Combine the masks for hexagon and grid cells

        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 500, activeLayerMask);

        if (hit.collider == null)
        {
            return;
        }

        // If a hexagon was clicked, proceed
        Hexagon clickedHexagon = hit.collider.GetComponent<Hexagon>();
        if (clickedHexagon != null)
        {
            curHexStack = clickedHexagon.hex;
            curHexStackInitialPos = curHexStack.transform.position;
        }
    }


    void DraggingAboveGround()
    {
        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 500, groundMask);
        if (hit.collider == null)
        {
            return;
        }

        Vector3 currentStackTargetPos = hit.point.With(y: 2);

        curHexStack.transform.position = Vector3.MoveTowards(
            curHexStack.transform.position, 
            currentStackTargetPos, 
            Time.deltaTime * 30);

        targetCell = null;
    }

    void DraggingAboveGridCell(RaycastHit hit)
    {
        GridCell gridCell = hit.collider.GetComponent<GridCell>();
        if (gridCell.isOccupied)
        {
            DraggingAboveGround();
        } else
        {
            DraggingAboveEmptyCell(gridCell);
        }
    }
    void DraggingAboveEmptyCell(GridCell gridCell)
    {
        Vector3 currentStackTargetPos = gridCell.transform.position.With(y: 2);

        curHexStack.transform.position = Vector3.MoveTowards(
            curHexStack.transform.position,
            currentStackTargetPos,
            Time.deltaTime * 30);

        targetCell = gridCell;
    }
    
    void ManageMouseUp()
    {
        if (targetCell == null)
        {
            curHexStack.transform.position = curHexStackInitialPos;
            curHexStack = null;
            return;
        }

        curHexStack.transform.position = targetCell.transform.position.With(y: 0.2f);
        curHexStack.transform.SetParent(targetCell.transform);
        curHexStack.Place();
        Color col = curHexStack.GetTopHexColor();
        Debug.Log($"color = {col}");

        targetCell.AssignStack(curHexStack);
        
        onStackPlaced?.Invoke(targetCell);
        targetCell = null;
        curHexStack = null;
    }
}

