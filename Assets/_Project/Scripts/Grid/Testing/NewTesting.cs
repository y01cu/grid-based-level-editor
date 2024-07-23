using UnityEngine;
using UnityEngine.EventSystems;

public class NewTesting : MonoBehaviour
{
    public static NewTesting Instance { get; private set; }

    public float cellSize
    {
        get => _cellSize;
        set => _cellSize = value;
    }

    public static TilemapGrid tilemapGrid { get; private set; }

    [SerializeField] private Camera camera;
    [SerializeField] private TilemapVisual tilemapVisual;

    private TilemapGrid.TilemapObject.TilemapSpriteTexture tilemapSpriteTexture;

    private TilemapGrid.TilemapObject.TilemapObjectType tilemapObjectType;

    private ObjectTypeSO tilemapObjectTypeSO;


    private float _cellSize = 4f;

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
        AdjustTypeButton.OnActiveBuildingTypeChanged += SetObjectTypeSO;
        AdjustSpriteButton.AdjustSpriteTexture += SetSprite;
        tilemapGrid = new TilemapGrid(4, 4, cellSize, Vector3.zero);
        tilemapGrid.SetTilemapVisualGrid(tilemapGrid, tilemapVisual);
    }

    private void SetObjectTypeSO(object sender, OnActiveBuildingTypeChangedEventArgs e)
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
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            tilemapGrid.SetupTilemapOnPositionWithSO(mouseWorldPosition, tilemapSpriteTexture, tilemapObjectTypeSO);
            ObjectGhost.Instance.SpawnPrefabOnPosition();
        }

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
        AdjustSpriteButton.AdjustSpriteTexture -= SetSprite;
        AdjustTypeButton.OnActiveBuildingTypeChanged -= SetObjectTypeSO;
    }
}