using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GridCell : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Hexagon hexPrefab;
    [OnValueChanged("GenerateHexagons")]
    [SerializeField] private Color[] colors;
    public HexStack Stack; //{ get; private set; }
    public bool isOccupied
    {
        get => Stack != null;
        private set { }
    }
    void Start()
    {
        if (transform.childCount > 1)
        {
            Stack = transform.GetChild(1).GetComponent<HexStack>();
            Stack.Initialize();

            foreach (Hexagon hex in Stack.hexagons)
            {
                hex.SetParent(Stack.transform);
            }
            
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
    private void GenerateHexagons()
    {
        while (transform.childCount > 1)
        {
            Transform t = transform.GetChild(1);
            t.SetParent(null);
            DestroyImmediate(t.gameObject);
        }

        Stack = new GameObject("Initial stack").AddComponent<HexStack>();
        Stack.transform.SetParent(this.transform, false);
        Stack.transform.localPosition = Vector3.up * .2f;

        for (int i = 0; i < colors.Length; i++)
        {
            Vector3 spawnPosition = Stack.transform.TransformPoint(Vector3.up * i * .2f);
            Hexagon hexInstance = Instantiate(hexPrefab, spawnPosition, Quaternion.Euler(0, 90, 0), Stack.transform);
            hexInstance.color = colors[i];
            Stack.Add(hexInstance);
        }

        // Explicitly initialize stack and trigger callback
        Stack.Initialize();
        StackController.onStackPlaced?.Invoke(this);
    }

    public void AssignStack(HexStack stack)
    {
        Stack = stack;
    }
    
}
