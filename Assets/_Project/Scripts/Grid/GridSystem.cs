using System;
using CodeMonkey.Utils;
using UnityEngine;


public class GridSystem<TGridObject>
{
    public event EventHandler<OnGridValueChangedEventArgs> OnGridObjectChanged;
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
    private TGridObject[,] gridArray;
    private Vector3 originPosition;

    public GridSystem(int width, int height, float cellSize, Vector3 originPosition, Func<GridSystem<TGridObject>, int, int, TGridObject> CreateGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = CreateGridObject(this, x, y);
            }
        }

        bool isDebugMode = false;
        if (isDebugMode)
        {
            TextMesh[,] debugTextArray = new TextMesh[width, height];
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 9, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            
            

            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

            OnGridObjectChanged += (sender, args) => { debugTextArray[args.x, args.y].text = gridArray[args.x, args.y]?.ToString(); };
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

    public void SetGridObject(int x, int y, TGridObject value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            Debug.Log($"value before: {gridArray[x, y]}");
            gridArray[x, y] = value;

            OnGridObjectChanged?.Invoke(this, new OnGridValueChangedEventArgs(x, y));
            Debug.Log($"value after: {gridArray[x, y]}");
        }
    }

    public void TriggerGridObjectChanged(int x, int y)
    {
        OnGridObjectChanged?.Invoke(this, new OnGridValueChangedEventArgs(x, y));
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        GridPosition gridPos = GetGridPosition(worldPosition);

        SetGridObject(gridPos.x, gridPos.y, value);
    }

    public TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }

        return default;
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        GridPosition gridPos = GetGridPosition(worldPosition);
        return GetGridObject(gridPos.x, gridPos.y);
    }
}