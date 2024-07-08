using System;
using CodeMonkey.Utils;
using UnityEngine;


public class GridSystem
{
    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public const int HEAT_MAP_MAX_VALUE = 100;
    public const int HEAT_MAP_MIN_VALUE = 0;

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

        bool isDebugMode = true;

        if (isDebugMode)
        {
            debugTextArray = new TextMesh[width, height];
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y].ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 12, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }

            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        }
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
            gridArray[x, y] = Mathf.Clamp(value, HEAT_MAP_MIN_VALUE, HEAT_MAP_MAX_VALUE);

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

    public void AddValue(int x, int y, int value)
    {
        SetValue(x, y, GetValue(x, y) + value);
    }

    public void AddValue(Vector3 worldPosition, int value, int fullValueRange, int totalRange)
    {
        int lowerValueAmount = Mathf.RoundToInt((float)value / (totalRange - fullValueRange));
        GridPosition gridPosition = GetGridPosition(worldPosition);
        for (int x = 0; x < totalRange; x++)
        {
            for (int y = 0; y < totalRange - x; y++)
            {
                int radius = x + y;
                int addValueAmount = value;
                if (radius > fullValueRange)
                {
                    addValueAmount -= lowerValueAmount * (radius - fullValueRange);
                }

                AddValue(gridPosition.x + x, gridPosition.y + y, addValueAmount);

                if (x != 0)
                {
                    AddValue(gridPosition.x - x, gridPosition.y + y, addValueAmount);
                }

                if (y != 0)
                {
                    AddValue(gridPosition.x + x, gridPosition.y - y, addValueAmount);
                    if (x != 0)
                    {
                        AddValue(gridPosition.x - x, gridPosition.y - y, addValueAmount);
                    }
                }
            }
        }
    }
}