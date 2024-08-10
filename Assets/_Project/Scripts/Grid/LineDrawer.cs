using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer<TGridObject> : MonoBehaviour
{
    public static void DrawHorizontalLine(GridSystem<TGridObject> gridSystem, int i)
    {
        GameObject emptyObject = new GameObject();
        LineRenderer lineRenderer = emptyObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.3f;
        lineRenderer.endWidth = 0.3f;
        lineRenderer.SetPosition(0, gridSystem.GetWorldPosition(i, 0));
        lineRenderer.SetPosition(1, gridSystem.GetWorldPosition(i, gridSystem.Length0));
        lineRenderer.material = Resources.Load<Material>("LineMaterial");
    }
    public static void DrawVerticalLine(GridSystem<TGridObject> gridSystem, int j)
    {
        GameObject emptyObject = new GameObject();
        LineRenderer lineRenderer = emptyObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.3f;
        lineRenderer.endWidth = 0.3f;
        lineRenderer.SetPosition(0, gridSystem.GetWorldPosition(0, j));
        lineRenderer.SetPosition(1, gridSystem.GetWorldPosition(gridSystem.Length1, j));
        lineRenderer.material = Resources.Load<Material>("LineMaterial");
    }

}
