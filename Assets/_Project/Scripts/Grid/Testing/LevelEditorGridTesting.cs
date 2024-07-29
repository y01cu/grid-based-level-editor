using System;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
public class LevelEditorGridTesting : MonoBehaviour
{
    public static EventHandler OnGridPositionChanged;
    public static LevelEditorGridTesting Instance { get; private set; }

    [field: SerializeField] public float cellSize { get; private set; }

    public static TilemapGrid tilemapGrid { get; private set; }

    [SerializeField] private Camera camera;
    [SerializeField] private TilemapVisual tilemapVisual;

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
        tilemapGrid = new TilemapGrid(4, 4, cellSize, Vector3.zero);
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

    private void Update()
    {
        Vector3 cameraToWorldPoint = camera.ScreenToWorldPoint(Input.mousePosition);
        IsOnGrid = tilemapGrid.gridSystem.GetGridObjectOnCoordinates(cameraToWorldPoint) != null;

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            tilemapGrid.SetupTilemapOnPositionWithSO(mouseWorldPosition, tilemapSpriteTexture, tilemapObjectTypeSO);
            ObjectGhost.Instance.SpawnAndAdjustPrefabOnPosition();
        }

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


        if (Input.GetKeyDown(KeyCode.S))
        {
            tilemapGrid.Save();
            Debug.Log("saved!");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            tilemapGrid.Load();
            Debug.Log("loaded!");
        }
    }

    private void OnDestroy()
    {
        // AdjustSpriteButton.AdjustSpriteTexture -= SetSprite;
        AdjustTypeButton.OnActiveObjectUpdated -= SetObjectTypeSO;
    }
}