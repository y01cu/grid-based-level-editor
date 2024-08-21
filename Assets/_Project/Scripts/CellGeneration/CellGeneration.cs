using System.Collections;
using UnityEngine;

public class CellGeneration : MonoBehaviour
{
    public static CellGeneration Instance { get; private set; }
    public int CellHeight
    {
        get => cellHeight;
        set => cellHeight = value;
    }

    [SerializeField] private int cellWidth;
    [SerializeField] private int cellHeight;

    private TilemapGrid tilemapGrid;

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

    private void Start()
    {
        CreateEmptyCells();
        tilemapGrid = new TilemapGrid(cellWidth, cellHeight, 3f, new Vector3(50, 50, 50), false);
        tilemapGrid.LoadWithSO();
    }

    private void CreateEmptyCells()
    {
        var emptyCell = Resources.Load("Cell Empty") as GameObject;
        for (int i = 0; i < cellWidth; i++)
        {
            for (int j = 0; j < cellHeight; j++)
            {
                var cellBase = Instantiate(emptyCell, new Vector3(i, j, 0.1f), Quaternion.Euler(270, 0, 0));
                cellBase.GetComponent<CellBase>().isEmpty = true;
            }
        }
    }
}