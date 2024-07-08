using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMapVisual : MonoBehaviour
{
    private GridSystem gridSystem;
    private Mesh mesh;
    private bool isMeshReadyToUpdate;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void SetGridSystem(GridSystem gridSystem)
    {
        this.gridSystem = gridSystem;
        UpdateHeapMapVisual();

        gridSystem.OnGridValueChanged += GridSystemOnOnGridValueChanged;
    }

    private void GridSystemOnOnGridValueChanged(object sender, GridSystem.OnGridValueChangedEventArgs e)
    {
        // UpdateHeapMapVisual();
        isMeshReadyToUpdate = true;
    }

    private void LateUpdate()
    {
        if (isMeshReadyToUpdate)
        {
            isMeshReadyToUpdate = false;
            UpdateHeapMapVisual();
        }
    }

    private void UpdateHeapMapVisual()
    {
        MeshUtils.CreateEmptyMeshArrays(gridSystem.Width * gridSystem.Height, out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for (int x = 0; x < gridSystem.Width; x++)
        {
            for (int y = 0; y < gridSystem.Height; y++)
            {
                int index = x * gridSystem.Height + y;

                Vector3 quadSize = new Vector3(1, 1) * gridSystem.CellSize;
                int gridValue = gridSystem.GetValue(x, y);
                int maxGridValue = 100;
                float gridValueNormalized = Mathf.Clamp01((float)gridValue / maxGridValue);
                Vector2 gridCellUV = new Vector2(gridValueNormalized, 0f);
                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, gridSystem.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, gridCellUV, gridCellUV);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}