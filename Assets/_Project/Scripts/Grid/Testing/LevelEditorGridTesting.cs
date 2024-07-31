using System;
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

    private TilemapGrid.TilemapObject.TilemapSpriteTexture tilemapSpriteTexture;
    private TilemapGrid.TilemapObject.TilemapObjectType tilemapObjectType;
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
        // AdjustSpriteButton.AdjustSpriteTexture += SetSprite;
        tilemapGrid = new TilemapGrid(width, height, cellSize, Vector3.zero);
        tilemapGrid.SetTilemapVisualGrid(tilemapGrid, tilemapVisual);
    }

    private void SetObjectTypeSO(object sender, OnActiveObjectTypeChangedEventArgs e)
    {
        tilemapObjectTypeSO = e.activeObjectTypeSO;
    }

    private void SetSprite(TilemapGrid.TilemapObject.TilemapSpriteTexture newTilemapSpriteTexture)
    {
        tilemapSpriteTexture = newTilemapSpriteTexture;
        Debug.Log($"updated to: {newTilemapSpriteTexture.ToString()}");
    }

    public void SetupObjectOnPosition(Vector3 mouseWorldPosition)
    {
        tilemapGrid.SetupTilemapOnPositionWithSO(mouseWorldPosition, tilemapSpriteTexture, tilemapObjectTypeSO);
    }

    private void Update()
    {
        Vector3 cameraToWorldPoint = camera.ScreenToWorldPoint(Input.mousePosition);
        IsOnGrid = tilemapGrid.gridSystem.GetGridObjectOnCoordinates(cameraToWorldPoint) != null;

        // Debug.Log($"IsOnGrid: {IsOnGrid} | obj: {tilemapGrid.gridSystem.GetGridObjectOnCoordinates(cameraToWorldPoint)}");

        Debug.Log($"obj on grid: {tilemapGrid.gridSystem.GetGridObjectOnCoordinates(cameraToWorldPoint)}");
        Debug.Log($"grid pos: {tilemapGrid.gridSystem.GetGridPosition(cameraToWorldPoint)}");

        if (currentGridPosition != tilemapGrid.gridSystem.GetGridPosition(cameraToWorldPoint).vector3With0Z)
        {
            currentGridPosition = tilemapGrid.gridSystem.GetGridPosition(cameraToWorldPoint)
                .vector3With0Z;

            if (IsOnGrid)
            {
                OnGridPositionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        // Debug.Log(tilemapGrid.gridSystem.GetGridObjectOnCoordinates(cameraToWorldPoint) == null ? "null" : "not null");
    }

    private void OnDestroy()
    {
        // AdjustSpriteButton.AdjustSpriteTexture -= SetSprite;
        AdjustTypeButton.OnActiveObjectUpdated -= SetObjectTypeSO;
    }
}