using System;
using UnityEngine;

public class ObjectGhost : MonoBehaviour
{
    public static ObjectGhost Instance { get; private set; }
    public GameObject prefab;

    public ObjectTypeSO objectTypeSO;

    [SerializeField] private Camera camera;
    private Transform spriteTransform;
    private GameObject activeGhostGameObject;

    private void Awake()
    {
        Instance = this;
        spriteTransform = transform.Find("Sprite");
        Hide();
    }

    private void AdjustTypeButton_OnActiveObjectUpdated(object sender, OnActiveObjectTypeChangedEventArgs e)
    {
        if (e.activeObjectTypeSO == null)
        {
            Hide();
        }
        else
        {
            SetObjectTypeSO(e.activeObjectTypeSO);
        }
        Destroy(activeGhostGameObject);
        CreateNewObjectFromSO();
        Show();
    }

    private void CreateNewObjectFromSO()
    {
        activeGhostGameObject = Instantiate(objectTypeSO.prefab.gameObject, spriteTransform.position, objectTypeSO.prefab.rotation);
        activeGhostGameObject.transform.SetParent(spriteTransform);
        activeGhostGameObject.transform.localScale = Vector3.one;
    }

    private void Start()
    {
        AdjustTypeButton.OnActiveObjectUpdated += AdjustTypeButton_OnActiveObjectUpdated;
        LevelEditorManager.OnGridPositionChanged += LevelEditorGridTesting_OnGridPositionChanged;
        var cellScale = LevelEditorManager.Instance.cellSize;
        spriteTransform.localScale = new Vector3(cellScale, cellScale, cellScale);
        ObjectPositioning.OnStartedRemovingObject += HideGhost;
        ObjectPositioning.OnEndedRemovingObject += ShowGhost;
    }

    private void HideGhost(object sender, EventArgs e)
    {
        spriteTransform.gameObject.SetActive(false);
    }

    private void ShowGhost(object sender, EventArgs e)
    {
        spriteTransform.gameObject.SetActive(true);
    }

    private void LevelEditorGridTesting_OnGridPositionChanged(object sender, EventArgs e)
    {
        // There was an audio clip playing here.
    }

    private void Update()
    {
        float cellSize = LevelEditorManager.Instance.cellSize;
        spriteTransform.position = LevelEditorManager.IsOnGrid ? LevelEditorManager.tilemapGrid.gridSystem
        .GetGridPosition(camera.ScreenToWorldPoint(Input.mousePosition)).vector3With0Z * cellSize + new Vector3(cellSize / 2, cellSize / 2, 0)
            : UtilsBase.GetMouseWorldPosition3OnCamera(camera);
    }

    public void SpawnAndAdjustPrefabOnPosition()
    {
        var spawnedPrefab = Instantiate(objectTypeSO.prefab, spriteTransform.position, activeGhostGameObject.transform.rotation);
        spawnedPrefab.transform.localScale *= LevelEditorManager.Instance.cellSize;
    }

    private void Hide()
    {
        spriteTransform.gameObject.SetActive(false);
    }

    private void Show()
    {
        spriteTransform.gameObject.SetActive(true);
    }

    private void SetObjectTypeSO(ObjectTypeSO newObjectTypeSO)
    {
        objectTypeSO = newObjectTypeSO;
    }

    public void RotateCurrentObjectWithAngle(Vector3 angle)
    {
        activeGhostGameObject.GetComponent<CellObject>().RotateByAngle(angle);
    }

    public Vector3 GetCurrentObjectRotation()
    {
        return activeGhostGameObject.transform.rotation.eulerAngles;
    }
}