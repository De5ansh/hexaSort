using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GridTester : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Elements")]
    [SerializeField] private Grid grid;

    [Header("Settings")]
    [OnValueChanged("GridPosUpdate")]
    [SerializeField] private Vector3Int gridPos;

    private void GridPosUpdate()
    {
        transform.position = grid.CellToWorld(gridPos);
    }
}
