using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEditorManager : MonoBehaviour
{
    public int LevelIndex { get => levelIndex; set => levelIndex = value; }
    public static EventHandler OnGridPositionChanged;
    public static LevelEditorManager Instance { get; private set; }
    public TextMeshProUGUI activeLevelText;
    public Transform parentOfCells;
    [field: SerializeField] public float cellSize { get; private set; }
    public static TilemapGrid tilemapGrid { get; private set; }
    [SerializeField] private Camera camera;
    [SerializeField] private TilemapVisual tilemapVisual;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip objectPlacedAudioClip;
    [SerializeField] private int width;
    [SerializeField] private int height;
    public Transform tileObjectParentTransform;
    private ObjectTypeSO tilemapObjectTypeSO;
    private Vector3 currentGridPosition = new();
    public static bool IsOnGrid { get; set; }

    public int currentTileObjectCount;
    /// <summary>
    /// Starts with 1
    /// </summary>
    private int levelIndex;

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
        SetupInitialValues();
        AdjustTypeButton.OnActiveObjectUpdated += SetObjectTypeSO;
        tilemapGrid = new TilemapGrid(width, height, cellSize, Vector3.zero, true);
        SavingSystem.SaveTemplate();
        SavingSystem.Load();
    }

    private void SetupInitialValues()
    {
        levelIndex = 1;
        activeLevelText.text = $"LEVEL {levelIndex}";
    }
    private void SetObjectTypeSO(object sender, OnActiveObjectTypeChangedEventArgs e)
    {
        tilemapObjectTypeSO = e.activeObjectTypeSO;
    }
    public void SetupObjectOnPosition(Vector3 mouseWorldPosition, Vector3 rotation)
    {
        tilemapGrid.SetupTilemapObject(mouseWorldPosition, rotation, tilemapObjectTypeSO);
    }

    private void Update()
    {
        Vector3 cameraToWorldPoint = camera.ScreenToWorldPoint(Input.mousePosition);
        IsOnGrid = tilemapGrid.gridSystem.GetGridObjectOnCoordinates(cameraToWorldPoint) != null;
        if (IsOnGrid)
        {
            currentTileObjectCount = tilemapGrid.gridSystem.GetGridObjectOnCoordinates(cameraToWorldPoint).GetObjectTypeSOList().Count;
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
    }

    [ContextMenu("Clear All and Save")]
    public void ClearAllAndSave()
    {
        // iterate through every tilemapobject and clear its lists

        for (int i = 0; i < tilemapGrid.gridSystem.Width; i++)
        {
            for (int j = 0; j < tilemapGrid.gridSystem.Height; j++)
            {
                tilemapGrid.gridSystem.GetGridObjectOnCoordinates(i, j).ClearAllLists();
            }
        }
        SavingSystem.Save();
        ClearAllTilesForNextLevel();
    }

    public void ChangeScene()
    {
        SavingSystem.Save();
        SceneManager.LoadScene(levelIndex - 1, LoadSceneMode.Single);
    }

    public void ClearAllTilesForNextLevel()
    {
        foreach (Transform cellChild in parentOfCells)
        {
            Destroy(cellChild.gameObject);
        }
    }

    private void OnDestroy()
    {
        AdjustTypeButton.OnActiveObjectUpdated -= SetObjectTypeSO;
    }
}