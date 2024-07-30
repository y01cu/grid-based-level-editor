using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectGhost : MonoBehaviour
{
    public static ObjectGhost Instance { get; private set; }

    [SerializeField] private Camera camera;

    private GameObject spriteGameObject;

    public GameObject prefab;

    private Vector3 tempPosition;
    private GameObject currentObject;

    private void Awake()
    {
        Instance = this;
        spriteGameObject = transform.Find("Sprite").gameObject;
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
            SetSprite(e.activeObjectTypeSO.spriteForLevelEditor);
            SetPrefab(e.activeObjectTypeSO.prefab.gameObject);
        }

        Destroy(currentObject);
        CreateNewObjectFromSO();

        Show();
    }

    private void CreateNewObjectFromSO()
    {
        currentObject = Instantiate(prefab, spriteGameObject.transform.position, Quaternion.Euler(90, 0, 0));
        currentObject.transform.SetParent(spriteGameObject.transform);
        currentObject.transform.localScale = Vector3.one;
    }

    private void Start()
    {
        tempPosition = spriteGameObject.transform.position;

        AdjustTypeButton.OnActiveObjectUpdated += AdjustTypeButtonOnActiveObjectUpdated;
        LevelEditorGridTesting.OnGridPositionChanged += LevelEditorGridTesting_OnGridPositionChanged;
        var cellScale = LevelEditorGridTesting.Instance.cellSize;
        spriteGameObject.transform.localScale = new Vector3(cellScale, cellScale, cellScale);
    }

    private void LevelEditorGridTesting_OnGridPositionChanged(object sender, EventArgs e)
    {
        // audioSource.PlayOneShot(hoverAudioClip);

        // if (IsGridHit())
        // {
        // }
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


        spriteGameObject.transform.position = LevelEditorGridTesting.IsOnGrid
            ? LevelEditorGridTesting.tilemapGrid.gridSystem
                  .GetGridPosition(camera.ScreenToWorldPoint(Input.mousePosition)).vector3With0Z * cellSize +
              new Vector3(cellSize / 2, cellSize / 2, 0)
            : UtilsBase.GetMouseWorldPosition3OnCamera(camera);


        // FindCustomObjectOnPointer();

        tempPosition = spriteGameObject.transform.position;
    }


    public void SpawnAndAdjustPrefabOnPosition()
    {
        var spawnedPrefab = Instantiate(prefab, spriteGameObject.transform.position, Quaternion.Euler(90, 0, 0));
        spawnedPrefab.transform.Translate(0, -2f, 0);
        spawnedPrefab.transform.localScale *= LevelEditorGridTesting.Instance.cellSize;
    }


    private void Hide()
    {
        spriteGameObject.SetActive(false);
    }

    private void Show()
    {
        spriteGameObject.SetActive(true);
    }


    private void SetSprite(Sprite newSprite)
    {
        spriteGameObject.GetComponent<SpriteRenderer>().sprite = newSprite;
        Show();
    }

    private void SetPrefab(GameObject newPrefab)
    {
        prefab = newPrefab;
    }

    private bool FindCustomObjectOnPointer()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        Debug.Log($"raycast results | {raycastResults.Count}");
        for (int i = 0; i < raycastResults.Count; i++)
        {
            if (raycastResults[i].gameObject.GetComponent<MeshFilter>() == null)
            {
                raycastResults.RemoveAt(i);
                i--;
            }
            else
            {
                Debug.Log("mesh filter found!", raycastResults[i].gameObject);
            }
        }

        return raycastResults.Count > 0;
    }
}