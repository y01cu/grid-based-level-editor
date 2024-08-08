using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorGridTesting : MonoBehaviour
{
    public static EventHandler OnGridPositionChanged;
    public static LevelEditorGridTesting Instance { get; private set; }

    [field: SerializeField] public float cellSize { get; private set; }

    public static TilemapGrid tilemapGrid { get; private set; }

    [SerializeField] private Camera camera;
    [SerializeField] private TilemapVisual tilemapVisual;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip objectPlacedAudioClip;
    [SerializeField] private int width;
    [SerializeField] private int height;

    private ObjectTypeSO tilemapObjectTypeSO;
    private Vector3 currentGridPosition = new();

    public static bool IsOnGrid { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        AdjustTypeButton.OnActiveObjectUpdated += SetObjectTypeSO;
        tilemapGrid = new TilemapGrid(width, height, cellSize, Vector3.zero);
        tilemapGrid.SetTilemapVisualGrid(tilemapGrid, tilemapVisual);
    }

    private void SetObjectTypeSO(object sender, OnActiveObjectTypeChangedEventArgs e)
    {
        tilemapObjectTypeSO = e.activeObjectTypeSO;
    }
    public void SetupObjectOnPosition(Vector3 mouseWorldPosition, Vector3 rotation)
    {
        var gridSystem = tilemapGrid.gridSystem;
        Vector3 cameraToWorldPoint = camera.ScreenToWorldPoint(Input.mousePosition);
        TilemapGrid.TilemapObject tilemapObject = gridSystem.GetGridObjectOnCoordinates(cameraToWorldPoint);

        tilemapGrid.SetupTilemapObject(mouseWorldPosition, rotation, tilemapObjectTypeSO);
    }

    private void Update()
    {
        Vector3 cameraToWorldPoint = camera.ScreenToWorldPoint(Input.mousePosition);
        IsOnGrid = tilemapGrid.gridSystem.GetGridObjectOnCoordinates(cameraToWorldPoint) != null;

        #region Logging Grid Position
        // Debug.Log($"IsOnGrid: {IsOnGrid} | obj: {tilemapGrid.gridSystem.GetGridObjectOnCoordinates(cameraToWorldPoint)}");
        // Debug.Log($"obj on grid: {tilemapGrid.gridSystem.GetGridObjectOnCoordinates(cameraToWorldPoint)}");
        // Debug.Log($"grid pos: {tilemapGrid.gridSystem.GetGridPosition(cameraToWorldPoint)}");
        #endregion

        if (currentGridPosition != tilemapGrid.gridSystem.GetGridPosition(cameraToWorldPoint).vector3With0Z)
        {
            currentGridPosition = tilemapGrid.gridSystem.GetGridPosition(cameraToWorldPoint)
                .vector3With0Z;

            if (IsOnGrid)
            {
                OnGridPositionChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void OnDestroy()
    {
        AdjustTypeButton.OnActiveObjectUpdated -= SetObjectTypeSO;
    }
}