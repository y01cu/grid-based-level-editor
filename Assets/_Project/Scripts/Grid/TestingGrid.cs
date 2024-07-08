using CodeMonkey.Utils;
using UnityEditor;
using UnityEngine;

public class TestingGrid : MonoBehaviour
{
    [SerializeField] private Camera camera;

    [SerializeField] private HeatMapVisual heatMapVisual;

    private GridSystem gridSystem;
    private float mouseMoveTimer;
    private float mouseMoveTimerMax = 0.01f;

    private void Start()
    {
        gridSystem = new GridSystem(100, 100, 2f, Vector3.zero);

        heatMapVisual.SetGridSystem(gridSystem);
    }

    private void Update()
    {
        // HandleHeatMapMouseMove();
        // HandleClickToModifyGrid();

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = camera.ScreenToWorldPoint(Input.mousePosition);
            gridSystem.AddValue(position, 100, 5, 40);
        }

        // if (Input.GetMouseButtonDown(1))
        // {
        //     Debug.Log(gridSystem.GetValue(camera.ScreenToWorldPoint(Input.mousePosition)));
        // }

        // Vector3 mousePos = new Vector3(Mouse.current.position.value.x, Mouse.current.position.value.y, 0);
        //
        // Debug.Log(gridSystem.GetGridPosition(mousePos));
    }

    private void HandleClickToModifyGrid()
    {
        if (Input.GetMouseButtonDown(0))
        {
            gridSystem.SetValue(camera.ScreenToWorldPoint(Input.mousePosition), 1);
        }
    }

    private void HandleHeatMapMouseMove()
    {
        mouseMoveTimer -= Time.deltaTime;
        if (mouseMoveTimer < 0f)
        {
            mouseMoveTimer += mouseMoveTimerMax;
            int gridValue = gridSystem.GetValue(camera.ScreenToWorldPoint(Input.mousePosition));
            gridSystem.SetValue(camera.ScreenToWorldPoint(Input.mousePosition), gridValue + 1);
        }
    }
}