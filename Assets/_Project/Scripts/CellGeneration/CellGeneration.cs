using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGeneration : MonoBehaviour
{
    public static CellGeneration Instance { get; private set; }

    public int Height
    {
        get => height;
        set => height = value;
    }

    public CellOrder[] cellOrders = new CellOrder[0];

    [SerializeField] private CellBase[] cellBases;
    [SerializeField] private Transform cellParentTransform;
    [SerializeField] private int width;
    [SerializeField] private int height;

    private Dictionary<Vector3, bool> pointsDictionary = new();
    private WaitForSeconds cellOrderDelay = new(0.2f);
    private const float LayerZAxisIncrement = -0.1f;

    [Serializable]
    public class CellOrder
    {
        public ObjectColor cellColor;
        public SpecificIndex frogIndex;
        public SpecificIndex arrowIndex;
        public Direction arrowDirection;
        public int columnIndex;
        public int rowIndex;
        public int stepCount;
        public OrderType orderType;
        public SpecificIndex finalBerryIndex;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private IEnumerator Start()
    {
        CellOrderProcessor cellOrderProcessor = new CellOrderProcessor(cellOrders, Height);
        foreach (var cellOrder in cellOrderProcessor.ProcessOrders())
        {
            CellObjectGenerator cellObjectGenerator = new CellObjectGenerator(cellBases, cellParentTransform, pointsDictionary, LayerZAxisIncrement, height);
            cellObjectGenerator.GenerateCellObjectsWithCellOrder(cellOrder);
            yield return cellOrderDelay;
        }
    }
}