using UnityEngine;

public static class CellObjectTypeSetup
{
    public static void SetCellObjectType(CellBase cellBase, CellGeneration.CellOrder cellOrder, int x, int y)
    {
        int index = cellOrder.orderType == OrderType.Column ? y : x;
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

    private static int GetIndex(SpecificIndex index, CellGeneration.CellOrder cellOrder)
    {
        return index switch
        {
            SpecificIndex.None => -1,
            SpecificIndex.First => cellOrder.orderType == OrderType.Column ? cellOrder.rowIndex : cellOrder.columnIndex,
            SpecificIndex.Last => cellOrder.orderType == OrderType.Column ? cellOrder.rowIndex + cellOrder.stepCount - 1 : cellOrder.columnIndex + cellOrder.stepCount - 1,
            _ => -1
        };
    }

    private static Vector3 GetArrowRotation(Direction direction)
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

    private static Vector3 GetProperRotation(CellGeneration.CellOrder cellOrder)
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
}
