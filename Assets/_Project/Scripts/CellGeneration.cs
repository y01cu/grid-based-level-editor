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
    }

    // Attention! Everytime a grid layer is set the position of the next layer must increase for it to be placed on previous one.

    [SerializeField] private int width;
    [SerializeField] private int height;

    [SerializeField] private CellBase[] cellBases;

    [SerializeField] private Transform cellParentTransform;

    private const float LayerZAxisIncrement = -0.1f;

    private const float XAxisAngle = -90;

    private Dictionary<Vector3, bool> Points = new();

    private WaitForSeconds cellOrderDelay = new(.1f);

    private IEnumerator Start()
    {
        // Fill base cells
        GenerateCellObjects(FrogPosition._NoFrog, CellBase.ObjectColor._Empty, 0, -1, false);
        GenerateCellObjects(FrogPosition._NoFrog, CellBase.ObjectColor._Empty, 1, -1, false);
        GenerateCellObjects(FrogPosition._NoFrog, CellBase.ObjectColor._Empty, 2, -1, false);
        GenerateCellObjects(FrogPosition._NoFrog, CellBase.ObjectColor._Empty, 3, -1, false);
        // ---
        yield return cellOrderDelay;
        GenerateCellObjects(FrogPosition.ColumnBottomFrog, CellBase.ObjectColor.Red, 1, -1, false);
        yield return cellOrderDelay;
        GenerateCellObjects(FrogPosition.ColumnTopFrog, CellBase.ObjectColor.Yellow, 2, -1, false);
        // yield return cellOrderDelay;
        // GenerateCellObjects(FrogPosition.ColumnBottomFrog, CellBase.ObjectColor.Blue, 1, -1, true);
        // yield return cellOrderDelay;
        // GenerateCellObjects(FrogPosition.ColumnBottomFrog, CellBase.ObjectColor.Green, 1, 3, false);
        // yield return cellOrderDelay;
    }

    public int GetHeight()
    {
        return height;
    }

    // TODO: Add how many cells will be created parameter
    [Command]
    private void GenerateCellObjects(FrogPosition frogPosition, CellBase.ObjectColor objectColor, int columnIndex, int stepCount, bool hasArrow)
    {
        switch (frogPosition)
        {
            case FrogPosition._NoFrog:
            {
                GenerateBaseCellObjects(columnIndex, stepCount, -1, hasArrow, OrderType.Column, objectColor);
                break;
            }

            case FrogPosition.ColumnBottomFrog:
            {
                GenerateBaseCellObjects(columnIndex, stepCount, 0, hasArrow, OrderType.Column, objectColor);
                break;
            }

            case FrogPosition.ColumnTopFrog:
            {
                GenerateBaseCellObjects(columnIndex, stepCount, height - 1, hasArrow, OrderType.Column, objectColor);
                break;
            }

            case FrogPosition.RowLeftFrog:
            {
                GenerateBaseCellObjects(columnIndex, stepCount, 0, hasArrow, OrderType.Row, objectColor);
                break;
            }

            case FrogPosition.RowRightFrog:
            {
                GenerateBaseCellObjects(columnIndex, stepCount, width - 1, hasArrow, OrderType.Row, objectColor);
                break;
            }
        }
    }

    private async void GenerateBaseCellObjects(int columnIndex, int stepCount, int frogIndex, bool hasArrow, OrderType orderType, CellBase.ObjectColor objectColor)
    {
        GameObject layerParentObject = new GameObject("L_" + "-C_" + objectColor);

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
                var showingBelowTransformation = 0.05f;
                // var targetPosition = new Vector3(x, y + (layerIndex * 0.05f), layerIndex * LayerZAxisIncrement);
                var targetPosition = CheckAndUpdateUpwardsIfNecessary(new Vector3(x, y, LayerZAxisIncrement));

                int layerValue = (int)(targetPosition.z / LayerZAxisIncrement);

                var newPosition = targetPosition + new Vector3(0, layerValue * showingBelowTransformation, 0);

                var targetRotation = Quaternion.Euler(XAxisAngle, 0, 0);
                var cellBase = Instantiate(cellBases[(int)objectColor], newPosition, targetRotation, layerParentObject.transform);

                // Vector3 cellBasePosition = cellBase.transform.position - new Vector3(0, 0, -0.6f);
                //
                // cellBase.transform.position = cellBasePosition;

                if (y == frogIndex)
                {
                    cellBase.ChangeCellObjectType(CellBase.ObjectType.Frog);
                    if (frogIndex == 0)
                    {
                        cellBase.additionalRotationVector3 = new Vector3(0, 180, 0);
                    }
                }

                if (y == stepCount - 1)
                {
                    if (hasArrow)
                    {
                        cellBase.ChangeCellObjectType(CellBase.ObjectType.Arrow);
                    }
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