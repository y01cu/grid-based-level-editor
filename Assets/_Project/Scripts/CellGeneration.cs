using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using QFSW.QC;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using VHierarchy.Libs;

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
    }

    // Attention! Everytime a grid layer is set the position of the next layer must increase for it to be placed on previous one.

    [SerializeField] private int width;
    [SerializeField] private int height;

    [SerializeField] private CellBase[] cellBases;

    [SerializeField] private Transform cellParentTransform;

    private const float LayerZAxisIncrement = -0.1f;

    private const float XAxisAngle = -90;

    private void Start()
    {
        // Fill base cells
        GenerateCellObjects(FrogPosition._NoFrog, CellBase.ObjectColor._Empty, 4, 0, -1);
        GenerateCellObjects(FrogPosition.ColumnBottomFrog, CellBase.ObjectColor.Blue, 0, 1, -1);
        GenerateCellObjects(FrogPosition.ColumnTopFrog, CellBase.ObjectColor.Red, 3, 1, -1);
    }

    public int GetHeight()
    {
        return height;
    }

    // TODO: Add how many cells will be created parameter
    [Command]
    private void GenerateCellObjects(FrogPosition frogPosition, CellBase.ObjectColor objectColor, int columnIndex, int layerIndex, int stepCount)
    {
        switch (frogPosition)
        {
            case FrogPosition._NoFrog:
            {
                GenerateCellObjectsWithoutFrog(columnIndex, layerIndex, stepCount, objectColor);
                break;
            }

            case FrogPosition.ColumnBottomFrog:
            {
                GenerateCellObjectsWithFrog(columnIndex, layerIndex, stepCount, 0, OrderType.Column, objectColor);
                break;
            }

            case FrogPosition.ColumnTopFrog:
            {
                GenerateCellObjectsWithFrog(columnIndex, layerIndex, stepCount, height - 1, OrderType.Column, objectColor);
                break;
            }

            case FrogPosition.RowLeftFrog:
            {
                GenerateCellObjectsWithFrog(columnIndex, layerIndex, stepCount, 0, OrderType.Row, objectColor);
                break;
            }

            case FrogPosition.RowRightFrog:
            {
                GenerateCellObjectsWithFrog(columnIndex, layerIndex, stepCount, width - 1, OrderType.Row, objectColor);
                break;
            }
        }
    }

    private void GenerateCellObjectsWithoutFrog(int columnIndex, int layerIndex, int stepCount, CellBase.ObjectColor objectColor)
    {
        GameObject layerParentObject = new GameObject("L_" + layerIndex + "-C_" + objectColor);

        layerParentObject.transform.SetParent(cellParentTransform);

        if (columnIndex == width)
        {
            Debug.Log("column index = width");
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var targetPosition = new Vector3(x, y, layerIndex * LayerZAxisIncrement);
                    var targetRotation = Quaternion.Euler(XAxisAngle, 0, 0);
                    var cellBase = Instantiate(cellBases[(int)objectColor], targetPosition, targetRotation);
                    cellBase.transform.SetParent(layerParentObject.transform);
                }
            }
        }
    }

    private async void GenerateCellObjectsWithFrog(int columnIndex, int layerIndex, int stepCount, int frogIndex, OrderType orderType, CellBase.ObjectColor objectColor)
    {
        GameObject layerParentObject = new GameObject("L_" + layerIndex + "-C_" + objectColor);

        layerParentObject.transform.SetParent(cellParentTransform);

        if (stepCount == -1)
        {
            stepCount = height;
        }

        if (orderType == OrderType.Column)
        {
            int x = columnIndex;

            for (int y = 0; y < stepCount; y++)
            {
                var targetPosition = new Vector3(x, y + (layerIndex * 0.05f), layerIndex * LayerZAxisIncrement);
                var targetRotation = Quaternion.Euler(XAxisAngle, 0, 0);
                var cellBase = Instantiate(cellBases[(int)objectColor], targetPosition, targetRotation, layerParentObject.transform);
                if (y == frogIndex)
                {
                    cellBase.ChangeCellObjectType(CellBase.ObjectType.Frog);
                    if (frogIndex == 0)
                    {
                        cellBase.additionalRotationVector3 = new Vector3(0, 180, 0);
                    }
                }

                // cellBase.CreateCellObject();

                await Task.Delay(50);
            }
        }
    }

    [Command]
    private void DeleteTopLayer()
    {
        Destroy(cellParentTransform.GetChild(cellParentTransform.childCount - 1).gameObject);
    }

    public enum FrogPosition
    {
        _NoFrog,
        ColumnTopFrog,
        ColumnBottomFrog,
        RowRightFrog,
        RowLeftFrog
    }

    public enum OrderType
    {
        Column,
        Row
    }
}