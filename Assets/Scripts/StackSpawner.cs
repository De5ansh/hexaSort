using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class StackSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Elements")]
    [SerializeField] private Transform stackParentPos;
    [SerializeField] private Hexagon hexagonPre;
    [SerializeField] private HexStack hexagonStackPre;

    [Header("Settings")]
    [NaughtyAttributes.MinMaxSlider(2,8)]
    [SerializeField] private Vector2Int minMaxCount;
    public Color[] colors;
    private int stackCounter;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        StackController.onStackPlaced += StackPlacedCallBack;
    }

    private void OnDestroy()
    {
        StackController.onStackPlaced -= StackPlacedCallBack;
    }

    private void StackPlacedCallBack(GridCell gridCell)
    {
        stackCounter++;
        if (stackCounter >= 3)
        {
            stackCounter = 0;
            GenerateStacks();
        }
    }
    void Start()
    {
        GenerateStacks();
    }

    private void GenerateStacks()
    {
        for (int i = 0; i < stackParentPos.childCount; i++)
        {
            GenerateStack(stackParentPos.GetChild(i));
        }
    }

    private void GenerateStack(Transform parent)
    {
        HexStack hexStack = Instantiate(hexagonStackPre, parent.position, Quaternion.identity, parent);
        hexStack.name = $"Stack { parent.GetSiblingIndex() }";
        

        int amount = Random.Range(minMaxCount.x, minMaxCount.y);
        int firstColorCount = Random.Range(0, amount);

        Color[] colorArray = GetRandomColors();
        for (int i = 0; i<amount; i++)
        {
            
            Vector3 hexagonLocalPos = Vector3.up * i * .2f;
            Vector3 spawnPosition = hexStack.transform.TransformPoint(hexagonLocalPos);
            
            Hexagon hexInstance = Instantiate(hexagonPre, spawnPosition, Quaternion.Euler(0, 90, 0), hexStack.transform);
            hexInstance.Configure(hexStack);
            hexInstance.color = i < firstColorCount ? colorArray[0]: colorArray[1];
            hexStack.Add(hexInstance);
        }

        
    }

    private Color[] GetRandomColors()
    {
        List<Color> colorList = new List<Color>();
        colorList.AddRange(colors);

        if (colorList.Count <= 0 )
        {
            return null;
        }

        Color first = colorList.OrderBy(x => Random.value).First();
        colorList.Remove(first);

        if (colorList.Count <= 0 )
        {
            return null;
        }

        Color second = colorList.OrderBy(x => Random.value).First();
        return new Color[] { first, second };
    }
}
