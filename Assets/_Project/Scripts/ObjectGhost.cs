using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class ObjectGhost : MonoBehaviour
{
    public static ObjectGhost Instance { get; private set; }
    private bool isObjectReadyToBePlaced;

    public bool IsObjectReadyToBePlaced { get => isObjectReadyToBePlaced; }

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

    private async void CreateNewObjectFromSO()
    {
        const int initialMaterialAssignmentDelayForPrecision = 100; // milliseconds
        await Task.Delay(initialMaterialAssignmentDelayForPrecision);
        // var baseCellSO = Resources.Load<ObjectTypeSO>("Cell");
        activeGhostGameObject = Instantiate(objectTypeSO.prefab.gameObject, spriteTransform.position, objectTypeSO.prefab.rotation);
        activeGhostGameObject.transform.SetParent(spriteTransform);
        activeGhostGameObject.transform.localScale = objectTypeSO.prefab.localScale;
        activeGhostGameObject.GetComponent<CellObject>().IsInLevelEditor = true;
        activeGhostGameObject.GetComponent<Renderer>().sharedMaterials[0] = objectTypeSO.normalMaterials[objectTypeSO.materialIndex];

    }

    private void Start()
    {
        AdjustTypeButton.OnActiveObjectUpdated += AdjustTypeButton_OnActiveObjectUpdated;
        LevelEditorManager.OnGridPositionChanged += LevelEditorGridTesting_OnGridPositionChanged;
        PanelObjectControl.OnTimeToHideObject += PanelObjectControl_OnTimeToHideObject;
        var cellScale = LevelEditorManager.Instance.cellSize;
        spriteTransform.localScale = new Vector3(cellScale, cellScale, cellScale);
        ObjectPositioning.OnStartedRemovingObject += HideGhost;
        ObjectPositioning.OnEndedRemovingObject += ShowGhost;
    }

    private void PanelObjectControl_OnTimeToHideObject(object sender, EventArgs e)
    {
        Hide();
        objectTypeSO = null;
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


    private void LateUpdate()
    {
        float cellSize = LevelEditorManager.Instance.cellSize;

        spriteTransform.position = LevelEditorManager.IsOnGrid ? Vector3.Lerp(spriteTransform.position, LevelEditorManager.tilemapGrid.gridSystem
        .GetGridPosition(camera.ScreenToWorldPoint(Input.mousePosition)).vector3With0Z * cellSize + new Vector3(cellSize / 2, cellSize / 2, 0), Time.deltaTime * 50)
            : UtilsBase.GetMouseWorldPosition3OnCamera(camera);


        if (spriteTransform.position != LevelEditorManager.tilemapGrid.gridSystem.GetGridPosition(camera.ScreenToWorldPoint(Input.mousePosition)).vector3With0Z * cellSize + new Vector3(cellSize / 2, cellSize / 2, 0))
        {
            isObjectReadyToBePlaced = false;
        }
        else
        {
            isObjectReadyToBePlaced = true;
        }

        // Vector3 targetPosition = LevelEditorManager.tilemapGrid.gridSystem
        // .GetGridPosition(camera.ScreenToWorldPoint(Input.mousePosition)).vector3With0Z * cellSize + new Vector3(cellSize / 2, cellSize / 2, 0);

        // if (LevelEditorManager.IsOnGrid)
        // {
        //     Vector3 newPosition = Vector3.Lerp(spriteTransform.position, targetPosition, Time.deltaTime * 20);
        //     isObjectReadyToBePlaced = Vector3.Distance(newPosition, targetPosition) < 0.01f;
        // }
        // else
        // {
        //     spriteTransform.position = UtilsBase.GetMouseWorldPosition3OnCamera(camera);
        // }

    }

    public void SpawnAndAdjustPrefabOnPosition(Vector3 objectRotation)
    {
        // if (!IsObjectReadyToBePlaced)
        // {
        //     return;
        // }

        var baseCellSO = Resources.Load<ObjectTypeSO>("Cell");
        var initialAngleForCamera = Quaternion.Euler(270, 0, 0);

        var spawnedCell = Instantiate(baseCellSO.prefab, spriteTransform.position, initialAngleForCamera);
        var cellBase = spawnedCell.GetComponent<CellBase>();
        cellBase.isInLevelEditor = true;
        cellBase.objectTypeSO = objectTypeSO;
        cellBase.objectTypeSO.prefab.GetComponent<CellObject>().spawnRotation = objectRotation;
        cellBase.cellObjectMaterialIndex = objectTypeSO.materialIndex;
        spawnedCell.transform.localScale *= LevelEditorManager.Instance.cellSize;
        spawnedCell.transform.DOScale(spawnedCell.transform.localScale, 0.5f).From(Vector3.zero);

        var renderer = spawnedCell.GetComponent<Renderer>();
        var materials = renderer.sharedMaterials;
        materials[0] = baseCellSO.normalMaterials[objectTypeSO.materialIndex];
        renderer.materials = materials;
        Debug.Log($"material 0 should be {baseCellSO.normalMaterials[objectTypeSO.materialIndex].name}");
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
        activeGhostGameObject.GetComponent<CellObject>().RotateByAngleInTheEditor(angle);
    }

    public Vector3 GetCurrentObjectRotation()
    {
        if (activeGhostGameObject == null)
        {
            return Vector3.zero;
        }
        return activeGhostGameObject.transform.rotation.eulerAngles;
    }
}