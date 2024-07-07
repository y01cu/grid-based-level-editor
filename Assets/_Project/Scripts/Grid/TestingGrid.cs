using CodeMonkey.Utils;
using UnityEditor;
using UnityEngine;

public class TestingGrid : MonoBehaviour
{
    [SerializeField] private Camera camera;

    private GridSystem gridSystem;
    private float mouseMoveTimer;
    private float mouseMoveTimerMax = .01f;


    private void Start()
    {
        gridSystem = new GridSystem(100, 100, 10f, new Vector3(0, 0));

        HeatMapVisual heatMapVisual = new HeatMapVisual(gridSystem, GetComponent<MeshFilter>());
    }

    private void Update()
    {
        HandleHeatMapMouseMove();
        HandleClickToModifyGrid();

        // if (Input.GetMouseButtonDown(0))
        // {
        //     gridSystem.SetValue(camera.ScreenToWorldPoint(Input.mousePosition), 56);
        // }

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

    private class HeatMapVisual
    {
        private GridSystem gridSystem;
        private Mesh mesh;

        public HeatMapVisual(GridSystem gridSystem, MeshFilter meshFilter)
        {
            this.gridSystem = gridSystem;

            mesh = new Mesh();
            meshFilter.mesh = mesh;

            UpdateHeapMapVisual();

            gridSystem.OnGridValueChanged += GridSystemOnOnGridValueChanged;
        }

        private void GridSystemOnOnGridValueChanged(object sender, GridSystem.OnGridValueChangedEventArgs e)
        {
            UpdateHeapMapVisual();
        }

        private void UpdateHeapMapVisual()
        {
            Vector3[] vertices;
            Vector2[] uv;
            int[] triangles;

            MeshUtils.CreateEmptyMeshArrays(gridSystem.Width * gridSystem.Height, out vertices, out uv, out triangles);

            for (int x = 0; x < gridSystem.Width; x++)
            {
                for (int y = 0; y < gridSystem.Height; y++)
                {
                    int index = x * gridSystem.Height + y;
                    Vector3 baseSize = new Vector3(1, 1) * gridSystem.CellSize;
                    int gridValue = gridSystem.GetValue(x, y);
                    int maxGridValue = 100;
                    float gridValueNormalized = Mathf.Clamp01((float)gridValue / maxGridValue);
                    Vector2 gridCellUV = new Vector2(gridValueNormalized, 0f);
                    MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, gridSystem.GetWorldPosition(x, y) + baseSize * .5f, 0f, baseSize, gridCellUV, gridCellUV);
                }
            }

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
        }
    }
}