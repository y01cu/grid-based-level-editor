using System.Collections;
using UnityEngine;

public class CellGeneration : MonoBehaviour
{
    public static CellGeneration Instance { get; private set; }

    public int Height
    {
        get => height;
        set => height = value;
    }

    [SerializeField] private int width;
    [SerializeField] private int height;

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

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(.2f);
        tilemapGrid = new TilemapGrid(width, height, 3f, Vector3.zero);
        tilemapGrid.LoadWithSO();
    }
}