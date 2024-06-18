using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using QFSW.QC;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class CellGeneration : MonoBehaviour
{
    public static CellGeneration Instance;

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


        // if (cellOrders == null || cellOrders.Length == 0)
        // {
        //     cellOrders = new CellOrder[cellOrders.Length];
        //     for (int i = 0; i < cellOrders.Length; i++)
        //     {
        //         cellOrders[i] = new CellOrder();
        //     }
        // }
    }

    [SerializeField] private int orderLength;

    // Attention! Everytime a grid layer is set the position of the next layer must increase for it to be placed on previous one.

    [SerializeField] private int width;
    [SerializeField] private int height;

    [SerializeField] private CellBase[] cellBases;

    [SerializeField] private Transform cellParentTransform;

    private const float LayerZAxisIncrement = -0.1f;

    private const float XAxisAngle = -90;

    private Dictionary<Vector3, bool> Points = new();

    private WaitForSeconds cellOrderDelay = new(.2f);

    public CellOrder[] cellOrders = new CellOrder[Instance.orderLength];

    [Serializable]
    public class CellOrder
    {
        public CellBase.ObjectColor cellColor;
        public SpecificIndex frogIndex;
        public SpecificIndex arrowIndex;
        public Direction arrowDirection;
        public int columnIndex;
        public int rowIndex;
        public int stepCount;
        public OrderType orderType;
    }

    private IEnumerator Start()
    {
        for (int i = 0; i < cellOrders.Length; i++)
        {
            if (cellOrders[i].frogIndex == SpecificIndex.None && cellOrders[i].frogIndex == cellOrders[i].arrowIndex)
            {
                Debug.LogError("ERROR: TWO OBJS IN THE SAME INDEX!");
            }

            if (cellOrders[i].arrowIndex == SpecificIndex.None)
            {
                cellOrders[i].arrowDirection = Direction._None;
            }

            GenerateCellObjects(cellOrders[i].orderType, cellOrders[i].frogIndex, cellOrders[i].cellColor, cellOrders[i].columnIndex, cellOrders[i].rowIndex, cellOrders[i].stepCount, cellOrders[i].arrowIndex, cellOrders[i].arrowDirection);
            yield return cellOrderDelay;
        }
    }

    public int GetHeight()
    {
        return height;
    }

    // TODO: Add how many cells will be created parameter
    [Command]
    private void GenerateCellObjects(OrderType orderType, SpecificIndex frogIndex, CellBase.ObjectColor objectColor, int columnIndex, int rowIndex, int stepCount, SpecificIndex arrowIndex, Direction arrowDirection)
    {
        if (stepCount == -1)
        {
            stepCount = height;
        }

        switch (orderType)
        {
            case OrderType.Column:
                switch (frogIndex)
                {
                    case SpecificIndex.None:
                        GenerateBaseCellObjects(columnIndex, rowIndex, stepCount, frogIndex, arrowIndex, orderType, objectColor, Vector3.zero, arrowDirection);
                        break;
                    case SpecificIndex.First:
                        GenerateBaseCellObjects(columnIndex, rowIndex, stepCount, frogIndex, arrowIndex, orderType, objectColor, Vector3.zero, arrowDirection);
                        break;
                    case SpecificIndex.Last:
                        GenerateBaseCellObjects(columnIndex, rowIndex, stepCount, frogIndex, arrowIndex, orderType, objectColor, Vector3.zero, arrowDirection);
                        break;
                }

                break;

            case OrderType.Row:
                switch (frogIndex)
                {
                    case SpecificIndex.None:
                        GenerateBaseCellObjects(columnIndex, rowIndex, stepCount, frogIndex, arrowIndex, orderType, objectColor, Vector3.zero, arrowDirection);
                        break;
                    case SpecificIndex.First:
                        GenerateBaseCellObjects(columnIndex, rowIndex, stepCount, frogIndex, arrowIndex, orderType, objectColor, new Vector3(0, 90, -90), arrowDirection);
                        break;
                    case SpecificIndex.Last:
                        GenerateBaseCellObjects(columnIndex, rowIndex, stepCount, frogIndex, arrowIndex, orderType, objectColor, Vector3.zero, arrowDirection);
                        break;
                }

                break;
        }

        // switch (frogIndex)
        // {
        // case FrogIndex.None:
        // {
        //     GenerateBaseCellObjects(columnIndex, rowIndex, stepCount, -1, hasArrow, OrderType.Column, objectColor, new Vector3(0, 90, -90), arrowDirection);
        //     break;
        // }
        //
        // case FrogIndex.First:
        // {
        //     GenerateBaseCellObjects(columnIndex, rowIndex, stepCount, 0, hasArrow, OrderType.Column, objectColor, new Vector3(0, 90, -90), arrowDirection);
        //     break;
        // }
        //
        // case FrogIndex.Last:
        // {
        //     GenerateBaseCellObjects(columnIndex, rowIndex, stepCount, stepCount - 1, hasArrow, OrderType.Column, objectColor, new Vector3(0, 90, -90), arrowDirection);
        //     break;
        // }

        // case FrogIndex.RowLeftFrog:
        // {
        //     GenerateBaseCellObjects(columnIndex, rowIndex, stepCount, 0, hasArrow, OrderType.Row, objectColor, new Vector3(0, 90, 90), arrowDirection);
        //     break;
        // }
        //
        // case FrogIndex.RowRightFrog:
        // {
        //     GenerateBaseCellObjects(columnIndex, rowIndex, stepCount, stepCount - 1, hasArrow, OrderType.Row, objectColor, new Vector3(0, 90, -90), arrowDirection);
        //     break;
        // }
        // }
    }

    private async void GenerateBaseCellObjects(int columnIndex, int rowIndex, int stepCount, SpecificIndex frogIndex, SpecificIndex arrowIndex, OrderType orderType, CellBase.ObjectColor objectColor, Vector3 rotation, Direction arrowDirection)
    {
        GameObject layerParentObject = new GameObject("L_" + "-C_" + objectColor);

        layerParentObject.transform.SetParent(cellParentTransform);

        if (stepCount == -1)
        {
            stepCount = height;
        }

        var arrowIndexValue = arrowIndex switch
        {
            SpecificIndex.None => -1,
            SpecificIndex.First => 0,
            SpecificIndex.Last => stepCount - 1,
        };

        var frogIndexValue = frogIndex switch
        {
            SpecificIndex.None => -1,
            SpecificIndex.First => 0,
            SpecificIndex.Last => stepCount - 1,
        };

        var showingBelowTransformation = 0.05f;
        if (orderType == OrderType.Column)
        {
            int x = columnIndex;

            for (int y = 0; y < stepCount; y++)
            {
                // var targetPosition = new Vector3(x, y + (layerIndex * 0.05f), layerIndex * LayerZAxisIncrement);
                var targetPosition = CheckAndUpdateUpwardsIfNecessary(new Vector3(x, y, LayerZAxisIncrement));

                int layerValue = (int)(targetPosition.z / LayerZAxisIncrement);

                var newPosition = targetPosition + new Vector3(0, layerValue * showingBelowTransformation, 0);

                var targetRotation = Quaternion.Euler(XAxisAngle, 0, 0);
                // var targetRotation = Quaternion.Euler(rotation);
                var cellBase = Instantiate(cellBases[(int)objectColor], newPosition, targetRotation, layerParentObject.transform);

                if (y == frogIndexValue)
                {
                    switch (frogIndex)
                    {
                        case SpecificIndex.None:
                            break;

                        case SpecificIndex.First:

                            if (y == 0)
                            {
                                cellBase.ChangeCellObjectType(CellBase.ObjectType.Frog);
                                cellBase.additionalRotationVector3 = new Vector3(0, 180, 0);
                            }

                            break;

                        case SpecificIndex.Last:

                            if (y == stepCount - 1)
                            {
                                cellBase.ChangeCellObjectType(CellBase.ObjectType.Frog);
                                cellBase.additionalRotationVector3 = new Vector3(0, 0, 0);
                            }


                            break;
                    }
                }

                if (y == arrowIndexValue)
                {
                    cellBase.ChangeCellObjectType(CellBase.ObjectType.Arrow);

                    Vector3 additionalRotation = arrowDirection switch
                    {
                        Direction._None => Vector3.zero,
                        Direction.Left => Vector3.zero,
                        Direction.Right => new Vector3(0, 0, 180),
                        Direction.Up => new Vector3(0, 0, 90),
                        Direction.Down => new Vector3(0, 0, 270)
                    };

                    cellBase.additionalRotationVector3 = additionalRotation;

                    cellBase.arrowDirection = arrowDirection;
                }

                //
                // if (y == frogIndexValue)
                // {
                //     cellBase.ChangeCellObjectType();
                // }

                // cellBase.CreateCellObject();

                await Task.Delay(50);
            }
        }
        else
        {
            int y = rowIndex;

            for (int x = 0; x < stepCount; x++)
            {
                // var targetPosition = new Vector3(x, y + (layerIndex * 0.05f), layerIndex * LayerZAxisIncrement);
                var targetPosition = CheckAndUpdateUpwardsIfNecessary(new Vector3(x, y, LayerZAxisIncrement));

                int layerValue = (int)(targetPosition.z / LayerZAxisIncrement);

                var newPosition = targetPosition + new Vector3(0, layerValue * showingBelowTransformation, 0);

                var targetRotation = Quaternion.Euler(XAxisAngle, 0, 0);
                var cellBase = Instantiate(cellBases[(int)objectColor], newPosition, targetRotation, layerParentObject.transform);

                // Vector3 cellBasePosition = cellBase.transform.position - new Vector3(0, 0, -0.6f);
                //
                // cellBase.transform.position = cellBasePosition;

                // if (x == frogIndex)
                // if (frogIndex != SpecificIndex.None)
                // {
                //     cellBase.ChangeCellObjectType(CellBase.ObjectType.Frog);
                //     if (frogIndex == 0)
                //     {
                //         cellBase.additionalRotationVector3 = new Vector3(0, 270, 0);
                //     }
                //     else
                //     {
                //         cellBase.additionalRotationVector3 = new Vector3(0, 90, 0);
                //     }
                // }


                // if (x == stepCount - 1)
                // {
                //     if (arrowIndex != SpecificIndex.None)
                //     {
                //         cellBase.ChangeCellObjectType(CellBase.ObjectType.Arrow);
                //     }
                // }


                if (x == frogIndexValue)
                {
                    switch (frogIndex)
                    {
                        case SpecificIndex.None:
                            break;

                        case SpecificIndex.First:

                            if (y == 0)
                            {
                                cellBase.ChangeCellObjectType(CellBase.ObjectType.Frog);
                                cellBase.additionalRotationVector3 = new Vector3(0, 270, 0);
                            }

                            break;

                        case SpecificIndex.Last:

                            if (x == stepCount - 1)
                            {
                                cellBase.ChangeCellObjectType(CellBase.ObjectType.Frog);
                                cellBase.additionalRotationVector3 = new Vector3(0, 90, 0);
                            }


                            break;
                    }
                }

                if (x == arrowIndexValue)
                {
                    cellBase.ChangeCellObjectType(CellBase.ObjectType.Arrow);

                    Vector3 additionalRotation = arrowDirection switch
                    {
                        Direction._None => Vector3.zero,
                        Direction.Left => Vector3.zero,
                        Direction.Right => new Vector3(0, 0, 180),
                        Direction.Up => new Vector3(0, 0, 90),
                        Direction.Down => new Vector3(0, 0, 270)
                    };

                    cellBase.additionalRotationVector3 = additionalRotation;

                    cellBase.arrowDirection = arrowDirection;
                }

                // cellBase.CreateCellObject();

                await Task.Delay(50);
            }
        }
    }

    // We can add the point the move it a bit forward to see what's below it.
    private Vector3 CheckAndUpdateUpwardsIfNecessary(Vector3 point)
    {
        if (Points.ContainsKey(point))
        {
            point += new Vector3(0, 0, LayerZAxisIncrement);
            return CheckAndUpdateUpwardsIfNecessary(point);
        }

        Points.Add(point, true);
        return point;
    }

    [Command]
    private void DeleteTopLayer()
    {
        Destroy(cellParentTransform.GetChild(cellParentTransform.childCount - 1).gameObject);
    }

    public enum SpecificIndex
    {
        None,
        Last,
        First,
    }

    public enum OrderType
    {
        Column,
        Row
    }
}

public enum Direction
{
    _None,
    Up,
    Down,
    Right,
    Left,
}