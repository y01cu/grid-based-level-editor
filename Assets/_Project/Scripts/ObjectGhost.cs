using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectGhost : MonoBehaviour
{
    public static ObjectGhost Instance { get; private set; }
    public GameObject prefab;

    [SerializeField] private Camera camera;
    private Transform spriteTransform;
    private GameObject activeGhostGameObject;

    private void Awake()
    {
        Instance = this;
        spriteTransform = transform.Find("Sprite");
        Hide();
    }

    private void AdjustTypeButtonOnActiveObjectUpdated(object sender, OnActiveObjectTypeChangedEventArgs e)
    {
        if (e.activeObjectTypeSO == null)
        {
            Hide();
        }
        else
        {
            SetPrefab(e.activeObjectTypeSO.prefab.gameObject);
        }
        Destroy(activeGhostGameObject);
        CreateNewObjectFromSO();
        Show();
    }

    private void CreateNewObjectFromSO()
    {
        activeGhostGameObject = Instantiate(prefab, spriteTransform.position, prefab.transform.rotation);
        activeGhostGameObject.transform.SetParent(spriteTransform);
        activeGhostGameObject.transform.localScale = Vector3.one;
    }

    private void Start()
    {
        AdjustTypeButton.OnActiveObjectUpdated += AdjustTypeButtonOnActiveObjectUpdated;
        LevelEditorGridTesting.OnGridPositionChanged += LevelEditorGridTesting_OnGridPositionChanged;
        var cellScale = LevelEditorGridTesting.Instance.cellSize;
        spriteTransform.localScale = new Vector3(cellScale, cellScale, cellScale);
        ObjectPositioning.OnRemovingObjectStarted += HideGhost;
        ObjectPositioning.OnRemovingObjectEnded += ShowGhost;
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

    private bool IsGridHit()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit);
        return hit.collider != null;
    }

    private void Update()
    {
        float cellSize = LevelEditorGridTesting.Instance.cellSize;
        spriteTransform.position = LevelEditorGridTesting.IsOnGrid ? LevelEditorGridTesting.tilemapGrid.gridSystem
        .GetGridPosition(camera.ScreenToWorldPoint(Input.mousePosition)).vector3With0Z * cellSize + new Vector3(cellSize / 2, cellSize / 2, 0)
            : UtilsBase.GetMouseWorldPosition3OnCamera(camera);
    }

    public void SpawnAndAdjustPrefabOnPosition()
    {
        var spawnedPrefab = Instantiate(prefab, spriteTransform.position, activeGhostGameObject.transform.rotation);
        spawnedPrefab.transform.localScale *= LevelEditorGridTesting.Instance.cellSize;
    }

    private void Hide()
    {
        spriteTransform.gameObject.SetActive(false);
    }

    private void Show()
    {
        spriteTransform.gameObject.SetActive(true);
    }

    private void SetPrefab(GameObject newPrefab)
    {
        prefab = newPrefab;
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