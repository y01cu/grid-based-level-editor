using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private const float XAxisAngle = -90;

    // Attention! Everytime a grid layer is set the position of the next layer must increase for it to be placed on previous one.

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
        foreach (var cellOrder in cellOrders)
        {
            if (cellOrder.frogIndex != SpecificIndex.None && cellOrder.frogIndex == cellOrder.arrowIndex)
            {
                Debug.LogWarning("TWO OBJS IN THE SAME INDEX!");
            }

            if (cellOrder.arrowIndex == SpecificIndex.None)
            {
                cellOrder.arrowDirection = Direction._None;
            }

            GenerateCellObjectsWithCellOrder(cellOrder);
            yield return cellOrderDelay;
        }
    }

    private void GenerateCellObjectsWithCellOrder(CellOrder cellOrder)
    {
        if (cellOrder.stepCount == -1)
        {
            cellOrder.stepCount = Height;
        }

        if (cellOrder.orderType == OrderType.Column)
        {
            GenerateColumnCellObjects(cellOrder);
        }
        else
        {
            GenerateRowCellObjects(cellOrder);
        }
    }

    private async void GenerateColumnCellObjects(CellOrder cellOrder)
    {
        GameObject layerParentObject = CreateLayerParent(cellOrder.cellColor);

        if (cellOrder.rowIndex + cellOrder.stepCount > Height)
        {
            Debug.LogError("Improper index: column filled");
            cellOrder.rowIndex = 0;
            cellOrder.stepCount = 4;
        }

        int x = cellOrder.columnIndex;

        for (int y = cellOrder.rowIndex; y < cellOrder.rowIndex + cellOrder.stepCount; y++)
        {
            Vector3 targetPosition = VectorHelper.CheckGivenDictionaryAndUpdateVector(new Vector3(x, y, LayerZAxisIncrement), pointsDictionary, LayerZAxisIncrement);
            int layerValue = (int)(targetPosition.z / LayerZAxisIncrement);
            Vector3 newPosition = targetPosition + new Vector3(0, layerValue * 0.05f, 0);
            Quaternion targetRotation = Quaternion.Euler(XAxisAngle, 0, 0);

            CellBase cellBase = Instantiate(cellBases[(int)cellOrder.cellColor], newPosition, targetRotation, layerParentObject.transform);
            cellBase.orderType = cellOrder.orderType;

            SetCellObjectType(cellBase, cellOrder, y);

            await Task.Delay(50);
        }
    }

    private async void GenerateRowCellObjects(CellOrder cellOrder)
    {
        GameObject layerParentObject = CreateLayerParent(cellOrder.cellColor);

        if (cellOrder.columnIndex + cellOrder.stepCount > Height)
        {
            Debug.LogError("Improper index: row filled");
            cellOrder.columnIndex = 0;
            cellOrder.stepCount = 4;
        }

        int y = cellOrder.rowIndex;

        for (int x = cellOrder.columnIndex; x < cellOrder.columnIndex + cellOrder.stepCount; x++)
        {
            Vector3 targetPosition = VectorHelper.CheckGivenDictionaryAndUpdateVector(new Vector3(x, y, LayerZAxisIncrement), pointsDictionary, LayerZAxisIncrement);
            int layerValue = (int)(targetPosition.z / LayerZAxisIncrement);
            Vector3 newPosition = targetPosition + new Vector3(0, layerValue * 0.05f, 0);
            Quaternion targetRotation = Quaternion.Euler(XAxisAngle, 0, 0);

            CellBase cellBase = Instantiate(cellBases[(int)cellOrder.cellColor], newPosition, targetRotation, layerParentObject.transform);
            cellBase.orderType = cellOrder.orderType;

            SetCellObjectType(cellBase, cellOrder, x);

            await Task.Delay(50);
        }
    }

    private void SetCellObjectType(CellBase cellBase, CellOrder cellOrder, int index)
    {
        if (index == GetIndex(cellOrder.frogIndex, cellOrder))
        {
            cellBase.ChangeCellObjectType(ObjectType.Frog);
            cellBase.additionalRotationVector3 = GetProperRotation(cellOrder);
        }

        if (index == GetIndex(cellOrder.arrowIndex, cellOrder))
        {
            cellBase.ChangeCellObjectType(ObjectType.Arrow);
            cellBase.additionalRotationVector3 = GetArrowRotation(cellOrder.arrowDirection);
            cellBase.arrowDirection = cellOrder.arrowDirection;
        }

        if (index == GetIndex(cellOrder.finalBerryIndex, cellOrder))
        {
            cellBase.SetAsSpawningFinalBerryForFrog();
        }
    }

    private Vector3 GetProperRotation(CellOrder cellOrder)
    {
        Vector3 rotation = Vector3.zero;
        if (cellOrder.orderType == OrderType.Column)
        {
            rotation = cellOrder.frogIndex switch
            {
                SpecificIndex.First => new Vector3(0, 180, 0),
                SpecificIndex.Last => Vector3.zero,
                SpecificIndex.None => Vector3.zero
            };
        }
        else
        {
            rotation = cellOrder.frogIndex switch
            {
                SpecificIndex.First => new Vector3(0, 270, 0),
                SpecificIndex.Last => new Vector3(0, 90, 0),
                SpecificIndex.None => Vector3.zero
            };
        }

        return rotation;
    }

    private int GetIndex(SpecificIndex index, CellOrder cellOrder)
    {
        return index switch
        {
            SpecificIndex.None => -1,
            SpecificIndex.First => cellOrder.orderType == OrderType.Column ? cellOrder.rowIndex : cellOrder.columnIndex,
            SpecificIndex.Last => cellOrder.orderType == OrderType.Column ? cellOrder.rowIndex + cellOrder.stepCount - 1 : cellOrder.columnIndex + cellOrder.stepCount - 1,
            _ => -1
        };
    }

    private Vector3 GetArrowRotation(Direction direction)
    {
        return direction switch
        {
            Direction.Left => Vector3.zero,
            Direction.Right => new Vector3(0, 0, 180),
            Direction.Up => new Vector3(0, 0, 90),
            Direction.Down => new Vector3(0, 0, 270),
            _ => Vector3.zero
        };
    }

    private GameObject CreateLayerParent(ObjectColor cellOrderCellColor)
    {
        GameObject layerParentObject = new GameObject($"L_C_{cellOrderCellColor}");
        layerParentObject.transform.SetParent(cellParentTransform);
        return layerParentObject;
    }
}