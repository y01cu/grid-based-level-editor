using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEditorManager : MonoBehaviour
{
    public int LevelIndex { get => levelIndex; set => levelIndex = value; }
    public static EventHandler OnGridPositionChanged;
    public static LevelEditorManager Instance { get; private set; }
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
        AdjustTypeButton.OnActiveObjectUpdated += SetObjectTypeSO;
        tilemapGrid = new TilemapGrid(width, height, cellSize, Vector3.zero, true);
        levelIndex = 1;
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

    public void ChangeScene()
    {
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        AdjustTypeButton.OnActiveObjectUpdated -= SetObjectTypeSO;
    }
}