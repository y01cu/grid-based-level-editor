using System.Collections;
using UnityEngine;

public class CellGeneration : MonoBehaviour
{
    public static CellGeneration Instance { get; private set; }

    public int levelIndex;
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
        tilemapGrid = new TilemapGrid(cellWidth, cellHeight, 3f, new Vector3(50, 50, 50), false);
        tilemapGrid.LoadWithSO();
    }
}