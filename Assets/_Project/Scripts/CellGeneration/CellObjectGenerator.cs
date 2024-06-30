using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CellObjectGenerator
{
    private CellBase[] cellBases;
    private Transform cellParentTransform;
    private Dictionary<Vector3, bool> pointsDictionary;
    private float layerZAxisIncrement;
    private int height;

    public CellObjectGenerator(CellBase[] cellBases, Transform cellParentTransform, Dictionary<Vector3, bool> pointsDictionary, float layerZAxisIncrement, int height)
    {
        this.cellBases = cellBases;
        this.cellParentTransform = cellParentTransform;
        this.pointsDictionary = pointsDictionary;
        this.layerZAxisIncrement = layerZAxisIncrement;
        this.height = height;
    }

    public async void GenerateCellObjectsWithCellOrder(CellGeneration.CellOrder cellOrder)
    {
        if (cellOrder.orderType == OrderType.Column)
        {
            await GenerateColumnCellObjects(cellOrder);
        }
        else
        {
            await GenerateRowCellObjects(cellOrder);
        }
    }

    private async Task GenerateColumnCellObjects(CellGeneration.CellOrder cellOrder)
    {
        GameObject layerParentObject = CreateLayerParent(cellOrder.cellColor);

        if (cellOrder.rowIndex + cellOrder.stepCount > height)
        {
            Debug.LogError("Improper index: column filled");
            cellOrder.rowIndex = 0;
            cellOrder.stepCount = 4;
        }

        int x = cellOrder.columnIndex;

        for (int y = cellOrder.rowIndex; y < cellOrder.rowIndex + cellOrder.stepCount; y++)
        {
            await GenerateCellObject(layerParentObject, cellOrder, x, y);
        }
    }

    private async Task GenerateRowCellObjects(CellGeneration.CellOrder cellOrder)
    {
        GameObject layerParentObject = CreateLayerParent(cellOrder.cellColor);

        if (cellOrder.columnIndex + cellOrder.stepCount > height)
        {
            Debug.LogError("Improper index: row filled");
            cellOrder.columnIndex = 0;
            cellOrder.stepCount = 4;
        }

        int y = cellOrder.rowIndex;

        for (int x = cellOrder.columnIndex; x < cellOrder.columnIndex + cellOrder.stepCount; x++)
        {
            await GenerateCellObject(layerParentObject, cellOrder, x, y);
        }
    }

    private async Task GenerateCellObject(GameObject layerParentObject, CellGeneration.CellOrder cellOrder, int x, int y)
    {
        Vector3 targetPosition = VectorHelper.CheckGivenDictionaryAndUpdateVector(new Vector3(x, y, layerZAxisIncrement), pointsDictionary, layerZAxisIncrement);
        int layerValue = (int)(targetPosition.z / layerZAxisIncrement);
        Vector3 newPosition = targetPosition + new Vector3(0, layerValue * 0.05f, 0);
        Quaternion targetRotation = Quaternion.Euler(-90, 0, 0);

        CellBase cellBase = Object.Instantiate(cellBases[(int)cellOrder.cellColor], newPosition, targetRotation, layerParentObject.transform);
        cellBase.orderType = cellOrder.orderType;

        CellObjectTypeSetter.SetCellObjectType(cellBase, cellOrder, x, y);

        await Task.Delay(50);
    }

    private GameObject CreateLayerParent(ObjectColor cellColor)
    {
        GameObject layerParentObject = new GameObject($"L_C_{cellColor}");
        layerParentObject.transform.SetParent(cellParentTransform);
        return layerParentObject;
    }
}