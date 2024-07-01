using System.Collections.Generic;
using UnityEngine;

public class CellOrderProcessor
{
    private CellGeneration.CellOrder[] cellOrders;
    private int height;

    public CellOrderProcessor(CellGeneration.CellOrder[] cellOrders, int height)
    {
        this.cellOrders = cellOrders;
        this.height = height;
    }

    public IEnumerable<CellGeneration.CellOrder> ProcessOrders()
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

            if (cellOrder.stepCount == -1)
            {
                cellOrder.stepCount = height;
            }

            yield return cellOrder;
        }
    }
}