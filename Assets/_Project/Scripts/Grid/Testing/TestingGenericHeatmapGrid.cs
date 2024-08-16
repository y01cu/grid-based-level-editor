using CodeMonkey.Utils;
using UnityEditor;
using UnityEngine;

public class TestingGenericHeatmapGrid : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private HeatMapVisual heatMapVisual;

    private GridSystem<StringGridObject> stringGridSystem;
    private GridSystem<HeatMapGridObject> gridSystem;
    private float mouseMoveTimer;
    private float mouseMoveTimerMax = 0.01f;

    private void Start()
    {
        // gridSystem = new GridSystem<HeatMapGridObject>(100, 100, 2f, Vector3.zero, (GridSystem<HeatMapGridObject> g, int x, int y) => new HeatMapGridObject(g, x, y));
        stringGridSystem = new GridSystem<StringGridObject>(100, 100, 2f, Vector3.zero, true, (GridSystem<StringGridObject> g, int x, int y) => new StringGridObject(g, x, y));
        // heatMapVisual.SetGridSystem(gridSystem);
        // heatMapVisual.SetGridSystem(gridSystem);
    }

    private void Update()
    {
        Vector3 position = camera.ScreenToWorldPoint(Input.mousePosition);
        // HandleHeatMapMouseMove();
        // HandleClickToModifyGrid();

        // if (Input.GetMouseButtonDown(0))
        // {
        //     HeatMapGridObject heatMapGridObject = gridSystem.GetGridObject(position);
        //     heatMapGridObject?.AddValue(5);
        //     // gridSystem.SetValue(position, true);
        //     // gridSystem.AddValue(position, 100, 5, 40);
        // }

        if (Input.GetKeyDown(KeyCode.A))
        {
            stringGridSystem.GetGridObjectOnCoordinates(position).AddLetter("A");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            stringGridSystem.GetGridObjectOnCoordinates(position).AddLetter("B");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            stringGridSystem.GetGridObjectOnCoordinates(position).AddLetter("C");
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            stringGridSystem.GetGridObjectOnCoordinates(position).AddNumber("1");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            stringGridSystem.GetGridObjectOnCoordinates(position).AddNumber("2");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            stringGridSystem.GetGridObjectOnCoordinates(position).AddNumber("3");
        }

        // if (Input.GetMouseButtonDown(1))
        // {
        //     Debug.Log(gridSystem.GetValue(camera.ScreenToWorldPoint(Input.mousePosition)));
        // }

        // Vector3 mousePos = new Vector3(Mouse.current.position.value.x, Mouse.current.position.value.y, 0);
        //
        // Debug.Log(gridSystem.GetGridPosition(mousePos));
    }

    // private void HandleClickToModifyGrid()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         gridSystem.SetValue(camera.ScreenToWorldPoint(Input.mousePosition), 1);
    //     }
    // }
    //
    // private void HandleHeatMapMouseMove()
    // {
    //     mouseMoveTimer -= Time.deltaTime;
    //     if (mouseMoveTimer < 0f)
    //     {
    //         mouseMoveTimer += mouseMoveTimerMax;
    //         int gridValue = gridSystem.GetValue(camera.ScreenToWorldPoint(Input.mousePosition));
    //         gridSystem.SetValue(camera.ScreenToWorldPoint(Input.mousePosition), gridValue + 1);
    //     }
    // }
}

public class HeatMapGridObject
{
    private const int MIN = 0;
    private const int MAX = 100;

    private GridSystem<HeatMapGridObject> gridSystem;
    private int x;
    private int y;
    private int value;

    public HeatMapGridObject(GridSystem<HeatMapGridObject> gridSystem, int x, int y)
    {
        this.gridSystem = gridSystem;
        this.x = x;
        this.y = y;
    }

    public void AddValue(int addition)
    {
        value += Mathf.Clamp(addition, MIN, MAX);
        gridSystem.TriggerGridObjectChanged(x, y);
    }

    public float GetValueNormalized()
    {
        return (float)value / MAX;
    }

    public override string ToString()
    {
        return value.ToString();
    }
}

public class StringGridObject
{
    private GridSystem<StringGridObject> gridSystem;
    private int x;
    private int y;

    private string letters;
    private string numbers;

    public StringGridObject(GridSystem<StringGridObject> gridSystem, int x, int y)
    {
        this.gridSystem = gridSystem;
        this.x = x;
        this.y = y;
        letters = "";
        numbers = "";
    }

    public void AddLetter(string letter)
    {
        letters += letter;
        gridSystem.TriggerGridObjectChanged(x, y);
    }

    public void AddNumber(string number)
    {
        numbers += number;
        gridSystem.TriggerGridObjectChanged(x, y);
    }

    public override string ToString()
    {
        return $"{letters} \n {numbers}";
    }
}