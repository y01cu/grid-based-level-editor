using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;


public class GridSystem
{
    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;

    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int y;

        public OnGridValueChangedEventArgs(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public int Width => width;
    public int Height => height;
    public float CellSize => cellSize;


    private int width;
    private int height;
    private float cellSize;
    private int[,] gridArray;
    private TextMesh[,] debugTextArray;
    private Vector3 originPosition;

    public GridSystem(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new int[width, height];
        debugTextArray = new TextMesh[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y].ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 10, Color.red, TextAnchor.MiddleCenter);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.red, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.red, 100f);
            }
        }

        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.red, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.red, 100f);
    }


    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y, 0) * cellSize + originPosition;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition((int)((worldPosition.x - originPosition.x) / cellSize), (int)((worldPosition.y - originPosition.y) / cellSize));
    }

    public void SetValue(int x, int y, int value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs(x, y));
            // debugTextArray[x, y].text = gridArray[x, y].ToString();
        }
    }

    public void SetValue(Vector3 worldPosition, int value)
    {
        GridPosition gridPos = GetGridPosition(worldPosition);

        SetValue(gridPos.x, gridPos.y, value);
    }

    public int GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }

        return 0;
    }

    public int GetValue(Vector3 worldPosition)
    {
        GridPosition gridPos = GetGridPosition(worldPosition);
        return GetValue(gridPos.x, gridPos.y);
    }
}